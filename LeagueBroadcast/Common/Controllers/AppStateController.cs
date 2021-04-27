using LCUSharp;
using LeagueBroadcast.ChampSelect.Data.LCU;
using LeagueBroadcast.ChampSelect.State;
using LeagueBroadcast.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Controllers
{
    class AppStateController : ITickable
    {
        public static EventHandler GameStart, GameLoad, GameStop, ChampSelectStart, ChampSelectStop;
        public static List<Summoner> summoners = new();

        private static AppStateController _instance;

        public AppStateController Instance
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

        public AppStateController()
        {
            if (_instance != null)
                return;

            mainCtx = (MainViewModel)BroadcastController.Instance.Main.DataContext;
        }

        public void EnableChampSelect()
        {
            ChampSelectStart += BroadcastHubController.Instance.EnterChampSelect;
            ChampSelectStop += ClientController.OnChampSelectExit;

            BroadcastController.Instance.PBController.Enable();
            Log.Info("PickBan Enabled");
        }

        public void DisableChampSelect()
        {
            if (BroadcastHubController.CurrentLeagueState == "InProgress")
            {
                ConfigController.Component.PickBan.IsActive = true;
                Log.Warn("Tried disabling Champ Select while active");
            }
            ChampSelectStart -= BroadcastHubController.Instance.EnterChampSelect;
            ChampSelectStop -= ClientController.OnChampSelectExit;
            BroadcastController.Instance.PBController.Disable();
            Log.Info("PickBan Disabled");
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
                BroadcastController.CurrentLeagueState = LeagueState.None;
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
            mainCtx.ConnectionStatus = ConnectionStatusViewModel.LCU;
            Log.Info($"Connected to League Client ({stopwatch.ElapsedMilliseconds} ms).");
            return api;
        }

        public void DoTick()
        {

        }

        public static Summoner GetSummonerById(int id)
        {
            return summoners.Single(summoner => summoner.summonerId == id);
        }

        public async void CheckLeagueRunning()
        {
            var gameData = await IngameController.LoLDataProvider.GetGameData();
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

