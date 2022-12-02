using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Common.Data.LCU;
using LeagueBroadcast.Common.Data.Pregame;
using LeagueBroadcast.Common.Data.Pregame.State;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Common.Tickable;
using LeagueBroadcast.Server.DataProvider;
using LeagueBroadcast.Server.Http.FrontendConnector;
using LeagueBroadcast.Utils.Log;
using System.Text.Json;
using static LeagueBroadcast.Server.Controller.PregameStateConverter;

namespace LeagueBroadcast.Server.Controller
{
    public class PregameController : ITickable
    {
        private static PregameController? _instance;
        public static PregameController Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new PregameController();
                }

                return _instance;
            }
        }

        public PregameState State { get; private set; }

        public List<Player> CachedSummoners { get; private set; }


        private bool _isEnabled;

        private bool _updatedThisTick;

        private long _lastTimer;

        private int _failedAttemptsToRetrieveSessionTimer = 0;
        private const int maxFailedAttempts = 5;

        private readonly PregameConnector _conntector;

        private readonly PickBanComponentConfig _config;


        public static PregameController Init()
        {
            if (ConfigController.Get<ComponentConfig>().PickBan.IsActive)
            {
                Instance.Enable();
                BroadcastClientEventHandler.LateInitComplete += async (s, e) => {
                    SessionTimer? t = await LeagueClientDataProvider.GetChampSelectSessionTimer();
                    if (t is not null)
                    {
                        Session session = await LeagueClientDataProvider.GetChampSelectSession();
                        Instance.ApplyNewState(session);
                        PregameControllerEventHandler.FireChampSelectStarted(new ChampSelectStartedEventArgs());

                    }
                };
            }

            return Instance;
        }


        private PregameController()
        {
            $"Init Pregame Controller".Info("Pregame");
            _conntector = new();

            _config = ConfigController.Get<ComponentConfig>().PickBan;

            _config.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "IsActive":
                        if (_config.IsActive)
                        {
                            Enable();
                        }
                        else
                        {
                            Disable();
                        }
                        break;
                    default:
                        break;
                }
            };

            State = new PregameState(new PreGameDisplayData());
            CachedSummoners = new();
        }

        private void Enable()
        {
            if (_isEnabled)
                return;
            _isEnabled = true;
            "Init Pregame Controller".UpdateStartupProgressText();
            $"Enabled Component".Info("Pregame");
            LeagueClientEventHandler.ChampSelectStart += LeagueClientEventHandler_ChampSelectStart;
            LeagueClientEventHandler.ChampSelectStop += LeagueClientEventHandler_ChampSelectEnded;

            LeagueClientEventHandler.ClientConnected += LeagueClientEventHandler_ClientConnected;
            LeagueClientEventHandler.ClientDisconnected += LeagueClientEventHandler_ClientDisconnected;
        }

        private void LeagueClientEventHandler_ClientDisconnected(object? sender, EventArgs e)
        {
            State.LeagueConnected = false;
        }

        private void LeagueClientEventHandler_ClientConnected(object? sender, LeagueConnectedEventArgs e)
        {
            State.LeagueConnected = true;
        }

        private void LeagueClientEventHandler_ChampSelectStart(object? sender, EventArgs e)
        {
            if (TickController.IsTicking(this))
            {
                return;
            }
            "Starting Champ Select".Info("Pregame");
            _ = TickController.AddTickable(0, this);
            BroadcastClientEventHandler.ConnectionStatus = ConnectionStatus.PreGame;
        }

        public void LeagueClientEventHandler_ChampSelectEnded(object? sender, EventArgs e)
        {
            if (State.ChampionSelectActive)
            {
                bool finished = State.Timer == 0 && _lastTimer == 0;
                string finishedText = finished ? "finished" : "ended early";
                $"ChampSelect {finishedText}!".Info("Pregame");
                BroadcastClientEventHandler.ConnectionStatus = finished ? ConnectionStatus.Ingame : ConnectionStatus.Connected;
                State.ChampionSelectActive = false;

                PregameControllerEventHandler.FireStateUpdate(new StateUpdateEventArgs(State), this);
                _ = TickController.RemoveTickable(this);
            }
        }

        private void Disable()
        {
            if (!_isEnabled)
                return;
            _isEnabled = false;
            $"Disabled Component".Info("Pregame");
            LeagueClientEventHandler.ChampSelectStart -= LeagueClientEventHandler_ChampSelectStart;
            LeagueClientEventHandler.ChampSelectStop -= LeagueClientEventHandler_ChampSelectEnded;
        }

        private async Task CacheSummoners(Session session)
        {
            //Clear to reset summoners before caching
            CachedSummoners.Clear();

            List<Cell> blueTeam = session.MyTeam;
            List<Cell> redTeam = session.TheirTeam;

            Dictionary<Cell, Task<string>> jobs = LeagueClientDataProvider.GetPlayersInTeam(blueTeam);
            jobs = jobs.Concat(LeagueClientDataProvider.GetPlayersInTeam(redTeam)).ToDictionary(x => x.Key, x => x.Value);
            List<Task<string>> completedJobs = jobs.Values.ToList();
            while (completedJobs.Any())
            {
                Task<string> finished = await Task.WhenAny(completedJobs);
                CachedSummoners.Add(JsonSerializer.Deserialize<Player>(await finished)!);
                _ = completedJobs.Remove(finished);
            }

            $"Cached {CachedSummoners.Count} summoners".Info("Pregame");
        }

        public async void DoTick()
        {
            //Update the timer since LCUSharp only fires event when champ select changes states
            //Obviously does not include timer update since that would ruin the point of events
            if (!_updatedThisTick)
            {

                SessionTimer? raw = await LeagueClientDataProvider.GetChampSelectSessionTimer();
                if (raw is null)
                {
                    "[ChampSelect] Tried retrieving pick ban timer while not active. Ignoring".Warn();

                    if (_failedAttemptsToRetrieveSessionTimer++ == maxFailedAttempts)
                    {
                        _ = TickController.RemoveTickable(this);
                        _failedAttemptsToRetrieveSessionTimer = 0;
                    }
                    return;
                }

                State.Timer = PregameStateConverter.ConvertTimer(raw);
                PregameControllerEventHandler.FireStateUpdate(new StateUpdateEventArgs(State), this);
            }

            _updatedThisTick = false;
        }


        public void ApplyNewState(Session? s)
        {
            if (s is null)
            {
                $"Could not apply new state".Error("Pregame");
                return;
            }
            CurrentSession newState = new(true, s);
            if (!State.ChampionSelectActive)
            {
                State.ChampionSelectActive = true;
                PregameControllerEventHandler.FireChampSelectStarted(new ChampSelectStartedEventArgs(), this);
                // Also cache information about summoners since this wont change
                Task t = CacheSummoners(newState.Session);
                t.Wait();
            }

            _lastTimer = State.Timer;

            PregameStateConversionOutput cleanedData = ConvertState(newState);

            CurrentAction currentActionBefore = State.GetCurrentAction();

            State.NewState(cleanedData);

            CurrentAction currentActionAfter = State.GetCurrentAction();

            if (!currentActionBefore.Equals(currentActionAfter))
            {
                CurrentAction action = State.RefreshAction(currentActionBefore);
                PregameControllerEventHandler.FireNewAction(new NewActionEventArgs(action), this);
            }
        }
    }
}
