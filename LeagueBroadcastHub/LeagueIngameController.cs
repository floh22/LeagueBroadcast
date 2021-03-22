using LeagueBroadcastHub;
using LeagueBroadcastHub.Session;
using System;
using System.Timers;
using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Server;
using System.IO;
using System.Diagnostics;
using LeagueBroadcastHub.Log;

namespace LeagueIngameServer
{
    class LeagueIngameController
    {
        public GameController gameController;

        public static bool currentlyIngame = false;
        public static bool paused = false;
        public static EventHandler gameStart;
        public static EventHandler gameStop;

        private Timer tickTimer;
        public static readonly int tickRate = 2;   //Ticks per second. Setting this to < 1 will cause issues!
        static readonly int checkIngameInterval = 5;
        private int currentInterval = 0;

        public static LeagueIngameController Instance;

        private DataDragon dataDragon;

        private Process frontEnd;

        public void Start()
        {
            PreInit();
            Init();
        }

        private void PreInit()
        {
            //Singleton
            if (Instance != null)
                return;
            Instance = this;

            gameController = new GameController();
            Logging.Info("Pre Init complete");
        }

        private void Init()
        {
            //Start Data Dragon
            dataDragon = new DataDragon();
            dataDragon.FinishedLoading += (sender, args) => {
                Logging.Info("Data Dragon init finished");
                //Start Frontend Webserver (HTTP/WS)
                var WebServer = new EmbedIOServer("localhost", 9001);

                tickTimer = new Timer { Interval = 1000 / tickRate };
                tickTimer.Elapsed += DoTick;

                tickTimer.Start();

                Logging.Info("Init complete");
                PostInit();

            };
        }

        private void PostInit()
        {
            //Check for game immediately
            CheckLeagueRunning();

            var frontEndLoc = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "frontend"), "ingame");
            if (Directory.Exists(frontEndLoc)) {
                frontEnd = CommandUtils.RunNPM("start", frontEndLoc);
            }

            Logging.Info("Post init Complete \nLeagueBroadcastHub loaded");
        }

        private void DoTick(object sender, EventArgs e)
        {
            if(!currentlyIngame)
            {
                if (++currentInterval >= checkIngameInterval * tickRate)
                {
                    currentInterval = 0;
                    CheckLeagueRunning();
                }

            }
            if(currentlyIngame)
            {
                gameController.DoTick();
            }
        }

        private async void CheckLeagueRunning()
        {
            var gameData = await gameController.LoLDataProvider.GetGameData();
            if (gameData == null)
                return;

            Logging.Info("League Game found");
            gameController.SetGameData(gameData);
            gameController.InitGameState();
            currentlyIngame = true;
            OnGameStart(EventArgs.Empty);
        }

        public void OnGameStart(EventArgs e)
        {
            gameStart?.Invoke(this, e);
        }

        public void OnGameStop(EventArgs e)
        {
            gameStop?.Invoke(this, e);
        }

        public void OnAppExit()
        {
            frontEnd.Kill();
        }
    }
}
