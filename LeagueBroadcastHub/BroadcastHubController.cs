using LeagueBroadcastHub.Session;
using System;
using System.Timers;
using LeagueBroadcastHub.Data;
using LeagueBroadcastHub.Server;
using System.IO;
using System.Diagnostics;
using LeagueBroadcastHub.Log;
using System.Collections.Generic;
using LeagueBroadcastHub.Data.Provider;
using LeagueBroadcastHub.Data.Client;
using Newtonsoft.Json;
using System.Linq;
using LeagueBroadcastHub.Data.Client.DTO;
using LeagueBroadcastHub;

namespace LeagueIngameServer
{
    class BroadcastHubController
    {
        public GameController gameController;
        public ClientController clientController;
        public StateController stateController;
        public ReplayController replayController;

        public ChampSelectConnector champSelectConnector;

        public static List<ITickable> ToTick;

        private Timer tickTimer;
        public static readonly int tickRate = 2;   //Ticks per second. Setting this to < 1 will cause issues!
        public static string CurrentLeagueState = "None";

        public static BroadcastHubController Instance;

        private DataDragon dataDragon;

        public static Config ClientConfig;

        public BroadcastHubController()
        {
            ToTick = new List<ITickable>();
        }

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

            ReadFrontendConfig();

            Logging.Info("Pre Init complete");
        }

        private void Init()
        {
            //Start Data Dragon
            dataDragon = new DataDragon();
            dataDragon.FinishedLoading += (sender, args) => {
                Logging.Info("Data Dragon init finished");

                Logging.Verbose("Init champ select controller");
                _ = new LeagueBroadcastHub.State.Client.State();
                Logging.Verbose("State generated");
                clientController = new ClientController();

                Logging.Verbose("Init ingame controller");
                gameController = new GameController();

                Logging.Verbose("Init state controller");
                stateController = new StateController(this);

                Logging.Verbose("Init replay controller");
                replayController = new ReplayController();

                //Start Frontend Webserver (HTTP/WS)
                var WebServer = new EmbedIOServer("localhost", 9001);

                tickTimer = new Timer { Interval = 1000 / tickRate };
                tickTimer.Elapsed += DoTick;

                Logging.Verbose($"Starting LBH with tickrate of {tickRate}tps");
                tickTimer.Start();

                Logging.Info("Init complete");
                PostInit();

            };
        }

        private void PostInit()
        {
            champSelectConnector = new ChampSelectConnector();

            //Start ticking state controller to detect league state
            ToTick.Add(stateController);

            Logging.Verbose("Checking for running Game");
            stateController.CheckLeagueRunning();

            Logging.Info("Post init Complete \nLeagueBroadcastHub loaded");
        }

        private void DoTick(object sender, EventArgs e)
        {
            //Thread safe iterator
            List<ITickable> tickNow = ToTick.GetRange(0, ToTick.Count);
            tickNow.ForEach(tickable => tickable.DoTick());
        }

        public void OnAppExit()
        {
            gameController.OnExit();
        }

        public void EnterChampSelect(object sender, EventArgs e)
        {
            if (ToTick.Contains(clientController))
                return;
            Logging.Verbose("Starting Client Controller tick");
            //Make sure client ticks first
            ToTick.Insert(0,clientController);
            //This should never be possible but lets be cautious, its league after all
            if(CurrentLeagueState.Equals("InProgress"))
            {
                ToTick.Remove(gameController);
            }
        }

        public void EnterIngame(object sender, EventArgs e)
        {
            if( ToTick.Contains(gameController))
                return;
            Logging.Verbose("Starting Ingame Controller tick");
            ToTick.Add(gameController);
            if(CurrentLeagueState.Equals("ChampSelect"))
            {
                ToTick.Remove(clientController);
            }
            CurrentLeagueState = "InProgress";
            gameController.InitGameState();
        }

        private void ReadFrontendConfig()
        {
            var assetPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            if(!File.Exists(assetPath + "\\config.json"))
            {
                WriteDefaultConfig();
            }

            ClientConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText(assetPath + "\\config.json"));
            if(ClientConfig.fileVersion == null)
            {
                //Convert config not created by this port
                Logging.Warn("Detected outdated file Format. Attempting to correct");
                ClientConfig.fileVersion = Config.FileVersion;
                WriteConfig("config.json", ClientConfig);
            } else if(ClientConfig.fileVersion != Config.FileVersion)
            {
                //For now reset and backup config, add updater if its ever needed
                Logging.Warn("Outdated config detected. Updating and backing up old config");
                WriteConfig($"config_v{ClientConfig.fileVersion}.json.backup", ClientConfig);
                WriteDefaultConfig();
            }
        }

        public static void WriteDefaultConfig()
        {
            ClientConfig = Config.CreateDefaultConfig();
            WriteConfig("config.json", ClientConfig);
        }

        public static void WriteConfig(string configName, Config config)
        {
            var assetPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            using var stream = File.CreateText($"{assetPath}\\{configName}");
            stream.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
            stream.Close();
            return;
        }

        public static void UpdateConfig()
        {
            var assetPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            using var stream = File.CreateText($"{assetPath}\\config.json");
            stream.Write(JsonConvert.SerializeObject(ClientConfig, Formatting.Indented));
            stream.Close();
            return;
        }
    }
}
