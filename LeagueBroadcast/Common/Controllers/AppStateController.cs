using LCUSharp;
using LCUSharp.Websocket;
using LeagueBroadcast.ChampSelect.Data.LCU;
using LeagueBroadcast.ChampSelect.StateInfo;
using LeagueBroadcast.MVVM.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Controllers
{
    class AppStateController : ITickable
    {
        public static EventHandler GameLoad, GameStop, ChampSelectStart, ChampSelectStop;
        public static EventHandler<Process> GameStart;
        public static List<Summoner> summoners = new();

        private static AppStateController _instance;

        public static AppStateController Instance
        {
            get { return GetInstance(); }
            set { _instance = value; }
        }

        public LeagueClientApi ClientAPI;

        private MainViewModel mainCtx;

        private static AppStateController GetInstance()
        {
            if (_instance == null)
                _instance = new();
            return _instance;
        }

        private AppStateController()
        {
            if (_instance != null)
                return;

            mainCtx = (MainViewModel)BroadcastController.Instance.Main.DataContext;

            InitConnection();

            if(ConfigController.Component.PickBan.IsActive)
            {
                EnableChampSelect();
            }
            if(ConfigController.Component.Ingame.IsActive)
            {
                EnableIngame();
            }
        }

        public void EnableChampSelect()
        {
            ChampSelectStart += BroadcastController.Instance.PBController.OnEnterPickBan;
            ChampSelectStop += BroadcastController.Instance.PBController.OnPickBanExit;

            BroadcastController.Instance.PBController.Enable();
            Log.Info("PickBan Enabled");
        }

        public void DisableChampSelect()
        {
            if (BroadcastController.CurrentLeagueState == "ChampSelect")
            {
                ConfigController.Component.PickBan.IsActive = true;
                Log.Warn("Tried disabling Champ Select while active");
            }
            ChampSelectStart -= BroadcastController.Instance.PBController.OnEnterPickBan;
            ChampSelectStop -= BroadcastController.Instance.PBController.OnPickBanExit;
            BroadcastController.Instance.PBController.Disable();
            Log.Info("PickBan Disabled");
        }

        public static void EnableIngame()
        {
            GameStart += BroadcastController.Instance.IGController.EnterIngame;
            Log.Info("Ingame Enabled");
        }

        public static void DisableIngame()
        {
            if (BroadcastController.CurrentLeagueState == "InProgress")
            {
                ConfigController.Component.Ingame.IsActive = true;
                Log.Warn("Tried disabling Ingame while active");
            }
            GameStart -= BroadcastController.Instance.IGController.EnterIngame;
            Log.Info("Ingame Disabled");
        }

        private async void InitConnection()
        {
            ClientAPI = await ConnectToClient();
            ClientAPI.Disconnected += async (e, a) =>
            {
                State.LeagueDisconnected();
                mainCtx.ConnectionStatus = ConnectionStatusViewModel.DISCONNECTED;
                Log.Info("Client Disconnected! Attempting to reconnect...");
                await ClientAPI.ReconnectAsync();
                State.LeagueConntected();
                BroadcastController.CurrentLeagueState = "None";
                mainCtx.ConnectionStatus = ConnectionStatusViewModel.LCU;
                Log.Info("Client Reconnected!");
            };
        }

        private async Task<LeagueClientApi> ConnectToClient()
        {
            Log.Info("Connecting to League Client");
            var stopwatch = Stopwatch.StartNew();
            var api = await LeagueClientApi.ConnectAsync();
            api.EventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", ClientStateChanged);
            api.EventHandler.Subscribe("/lol-champ-select/v1/session", ChampSelectChanged);
            api.EventHandler.Subscribe("/lol-champ-select/vi/sfx-notifications", ChampSelectSFXChanged);
            stopwatch.Stop();
            State.LeagueConntected();
            if(mainCtx.ConnectionStatus != ConnectionStatusViewModel.CONNECTED)
                mainCtx.ConnectionStatus = ConnectionStatusViewModel.LCU;
            Log.Info($"Connected to League Client in {stopwatch.ElapsedMilliseconds} ms");
            return api;
        }

        private void ClientStateChanged(object sender, LeagueEvent e)
        {
            string eventType = e.Data.ToString();
            Log.Verbose($"New League State: {eventType}");
            switch (eventType)
            {
                case "ChampSelect":
                    ChampSelectStart?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
            if (!eventType.Equals("InProgress") && BroadcastController.CurrentLeagueState.Equals("InProgress"))
            {
                GameStop?.Invoke(this, EventArgs.Empty);
            }
            if (!eventType.Equals("ChampSelect") && BroadcastController.CurrentLeagueState.Equals("ChampSelect"))
            {
                ChampSelectStop?.Invoke(this, EventArgs.Empty);
            }
            BroadcastController.CurrentLeagueState = eventType;
        }

        private void ChampSelectChanged(object sender, LeagueEvent e)
        {
            if (ConfigController.Component.PickBan.IsActive)
                BroadcastController.Instance.PBController.ApplyNewState(e);
        }

        private void ChampSelectSFXChanged(object sender, LeagueEvent e)
        {
            if (ConfigController.Component.PickBan.IsActive)
                Log.Verbose($"SFX Change: {e.Data}");
        }

        public static async Task CacheSummoners(ChampSelect.Data.LCU.Session session)
        {
            //Clear to reset summoners before caching
            summoners.Clear();

            List<Cell> blueTeam = session.myTeam;
            List<Cell> redTeam = session.theirTeam;

            Dictionary<Cell, Task<string>> jobs = FetchPlayersFromTeam(blueTeam);
            jobs = jobs.Concat(FetchPlayersFromTeam(redTeam)).ToDictionary(x => x.Key, x => x.Value);
            var completedJobs = jobs.Values.ToList();
            while (completedJobs.Any())
            {
                Task<string> finished = await Task.WhenAny(completedJobs);
                summoners.Add(JsonConvert.DeserializeObject<Summoner>(await finished));
                completedJobs.Remove(finished);
            }
        }

        public static async Task<Timer> GetTimer()
        {
            return JsonConvert.DeserializeObject<Timer>(await Instance.ClientAPI.RequestHandler.GetJsonResponseAsync(HttpMethod.Get, $"/lol-champ-select/v1/session/timer"));
        }

        private static Dictionary<Cell, Task<string>> FetchPlayersFromTeam(List<Cell> team)
        {
            var toFinish = new Dictionary<Cell, Task<string>>();
            team.ForEach(cell => {
                try
                {
                    toFinish.Add(cell, Instance.ClientAPI.RequestHandler.GetJsonResponseAsync(HttpMethod.Get, $"lol-summoner/v1/summoners/{cell.summonerId}"));
                }
                catch (Exception)
                {
                    Log.Verbose("Could not fetch players for team. Is this not a custom game?");
                }

            });

            return toFinish;
        }

        public void DoTick()
        {

        }

        public static Summoner GetSummonerById(int id)
        {
            return summoners.Single(summoner => summoner.summonerId == id);
        }

    }

    public class StateControllerSettings
    {
        public bool PickBan;
        public bool Delay;
        public bool Ingame;
    }
}

