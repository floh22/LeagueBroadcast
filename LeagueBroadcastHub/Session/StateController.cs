using LCUSharp;
using LCUSharp.Websocket;
using LeagueBroadcastHub.Data.Client.LCU;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.State.Client;
using LeagueIngameServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Session
{
    class StateController : ITickable
    {

        public static EventHandler GameStart, GameStop, ChampSelectStart, ChampSelectStop;

        private BroadcastHubController BroadcastHub;

        public static LeagueClientApi ClientAPI;

        public static List<Summoner> summoners = new List<Summoner>();

        public static StateControllerSettings CurrentSettings = new StateControllerSettings();

        public StateController(BroadcastHubController main)
        {
            InitConnection();
            BroadcastHub = main;
            if(ActiveSettings._useIngame)
            {
                EnableIngame();
                CurrentSettings.Ingame = true;
            }
            

            if(ActiveSettings._usePickBan)
            {
                EnableChampSelect();
                CurrentSettings.PickBan = true;
            } 
        }

        public static void EnableChampSelect()
        {
            ChampSelectStart += BroadcastHubController.Instance.EnterChampSelect;
            ChampSelectStop += ClientController.OnChampSelectExit;
            Logging.Info("PickBan Enabled");
        }

        public static void DisableChampSelect()
        {
            if(BroadcastHubController.CurrentLeagueState == "InProgress")
            {
                ActiveSettings._usePickBan = true;
                Logging.Warn("Tried disabling Champ Select while active");
            }
            ChampSelectStart -= BroadcastHubController.Instance.EnterChampSelect;
            ChampSelectStop -= ClientController.OnChampSelectExit;
            Logging.Info("PickBan Disabled");
        }

        public static void EnableIngame()
        {
            GameStart += BroadcastHubController.Instance.EnterIngame;
            Logging.Info("Ingame Enabled");
        }

        public static void DisableIngame()
        {
            if(BroadcastHubController.CurrentLeagueState == "InProgress")
            {
                ActiveSettings._useIngame = true;
                Logging.Warn("Tried disabling Ingame while active");
            }
            GameStart -= BroadcastHubController.Instance.EnterIngame;
            Logging.Info("Ingame Disabled");
        }

        private async void InitConnection()
        {
            ClientAPI = await ConnectToClient();
            ClientAPI.Disconnected += async (e, a) =>
            {
                State.Client.State.LeagueDisconnected();
                Logging.Info("Client Disconnected! Attempting to reconnect...");
                await ClientAPI.ReconnectAsync();
                State.Client.State.LeagueConntected();
                Logging.Info("Client Reconnected!");
            };
        }

        private async Task<LeagueClientApi> ConnectToClient()
        {
            Logging.Info("Connecting to League Client");
            var stopwatch = Stopwatch.StartNew();
            var api = await LeagueClientApi.ConnectAsync();
            api.EventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", ClientStateChanged);
            api.EventHandler.Subscribe("/lol-champ-select/v1/session", ChampSelectChanged);
            api.EventHandler.Subscribe("/lol-champ-select/vi/sfx-notifications", ChampSelectSFXChanged);
            stopwatch.Stop();
            State.Client.State.LeagueConntected();
            Logging.Info($"Connected to League Client ({stopwatch.ElapsedMilliseconds} ms).");
            return api;
        }

        private void ClientStateChanged(object sender, LeagueEvent e)
        {
            string eventType = e.Data.ToString();
            Logging.Verbose($"New League State: {eventType}");
            switch(eventType)
            {
                case "ChampSelect":
                    ChampSelectStart?.Invoke(this, EventArgs.Empty);
                    break;
                case "InProgress":
                    GameStart?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
            if(!eventType.Equals("InProgress") && BroadcastHubController.CurrentLeagueState.Equals("InProgress"))
            {
                GameStop?.Invoke(this, EventArgs.Empty);
            }
            if(!eventType.Equals("ChampSelect") && BroadcastHubController.CurrentLeagueState.Equals("ChampSelect"))
            {
                ChampSelectStop?.Invoke(this, EventArgs.Empty);
            }
            BroadcastHubController.CurrentLeagueState = eventType;
        }

        private void ChampSelectChanged(object sender, LeagueEvent e)
        {
            if(CurrentSettings.PickBan)
                BroadcastHub.clientController.ApplyNewState(e);
        }

        private void ChampSelectSFXChanged(object sender, LeagueEvent e)
        {
            if(CurrentSettings.PickBan)
                Logging.Verbose($"SFX Change: {e.Data}");
        }

        public static async Task CacheSummoners(Data.Client.LCU.Session session)
        {
            //Clear to reset summoners before caching
            summoners.Clear();

            List<Cell> blueTeam = session.myTeam;
            List<Cell> redTeam = session.theirTeam;

            Dictionary<Cell, Task<string>> jobs = FetchPlayersFromTeam(blueTeam);
            jobs = jobs.Concat(FetchPlayersFromTeam(redTeam)).ToDictionary(x => x.Key, x => x.Value);
            var completedJobs = jobs.Values.ToList();
            while(completedJobs.Any())
            {
                Task<string> finished = await Task.WhenAny(completedJobs);
                summoners.Add(JsonConvert.DeserializeObject<Summoner>(await finished));
                completedJobs.Remove(finished);
            }
        }

        public static async Task<Timer> GetTimer()
        {
            return JsonConvert.DeserializeObject<Timer>(await ClientAPI.RequestHandler.GetJsonResponseAsync(HttpMethod.Get, $"/lol-champ-select/v1/session/timer"));
        }
        
        private static Dictionary<Cell, Task<string>> FetchPlayersFromTeam(List<Cell> team)
        {
            var toFinish = new Dictionary<Cell, Task<string>>();
            team.ForEach(cell => {
                try
                {
                    toFinish.Add(cell, ClientAPI.RequestHandler.GetJsonResponseAsync(HttpMethod.Get, $"lol-summoner/v1/summoners/{cell.summonerId}"));
                } catch(Exception)
                {
                    Logging.Verbose("Could not fetch players for team. Is this not a custom game?");
                }

            });

            return toFinish;
        }

        public static Summoner GetSummonerById(int id)
        {
            return summoners.Single(summoner => summoner.summonerId == id);
        }

        public void DoTick()
        {
            if(BroadcastHubController.CurrentLeagueState != "InProgress")
            {
                CheckLeagueRunning();
            }
        }

        private async void CheckLeagueRunning()
        {
            var gameData = await BroadcastHub.gameController.LoLDataProvider.GetGameData();
            if (gameData == null)
                return;

            GameStart?.Invoke(this, EventArgs.Empty);
        }
    }

    public class StateControllerSettings
    {
        public bool PickBan;
        public bool Delay;
        public bool Ingame;
    }
}
