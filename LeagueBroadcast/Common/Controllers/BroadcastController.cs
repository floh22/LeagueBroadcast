using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.Http;
using LeagueBroadcast.MVVM.View;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static LeagueBroadcast.OperatingSystem.Log;

namespace LeagueBroadcast.Common.Controllers
{
    class BroadcastController
    {
        public static string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static int TickRate = 2;
        public static string CurrentLeagueState = "None";
        public static BroadcastController Instance => GetInstance();
        public static EventHandler EarlyInitComplete, InitComplete, PostInitComplete;

        private static BroadcastController _instance;

        public ConfigController CfgController;
        public PickBanController PBController;
        public IngameController IGController;
        public AppStateController AppStController;
        public ReplayAPIController ReplayController;
        public DataDragon DDragon;
        public PickBanConnector PBConnector;

        public List<ITickable> ToTick;

        public MainWindow Main { get; private set; }
        public StartupWindow Startup { get; private set; }


        private Timer tickTimer;
        private BroadcastController()
        {
            DateTime loadStart = DateTime.Now;
            DateTime initFinish = DateTime.Now;
            EarlyInitComplete += (s, e) => {
                Log.Info($"Early Init Complete in {(DateTime.Now - loadStart).TotalMilliseconds}ms");
                initFinish = DateTime.Now;
            };
            InitComplete += (s, e) => { 
                Log.Info($"Init Complete in {(DateTime.Now - initFinish).ToString(@"s\.fff")}s");
                initFinish = DateTime.Now;
            };
            PostInitComplete += (s, e) => { 
                Log.Info($"Post Init Complete in {(DateTime.Now - initFinish).TotalMilliseconds}ms");
                Log.Info($"Total Startup time: {(DateTime.Now - loadStart).ToString(@"s\.fff")}s");
            };
            EarlyInit();
        }

        private static BroadcastController GetInstance()
        {
            if (_instance == null)
                _instance = new();
            return _instance;
        }

        private void StatusUpdate(string Status)
        {
            Log.Info(Status);
            Startup.GETDataContext().Status = Status;
        }

        private void EarlyInit()
        {
            DataDragon.FinishLoading += (s, e) => Init();
            InitComplete += (s, e) => PostInit();

            Startup = new();
            Startup.Show();

            ToTick = new();

            _ = new Log(LogLevel.Verbose);
            CfgController = ConfigController.Instance;
            DDragon = DataDragon.Instance;

            EarlyInitComplete?.Invoke(this, EventArgs.Empty);
        }

        private void Init()
        {
            Log.Info("DDragon loaded");
            StatusUpdate("Loading PickBan Controller");
            PBController = new();

            StatusUpdate("Loading Ingame Controller");
            IGController = new();

            StatusUpdate("Loading Replay Controller");
            ReplayController = new();

            StatusUpdate("Loading State Controller");
            AppStController = new();

            StatusUpdate("Loading Frontend Webserver (HTTP/WS)");
            var WebServer = new EmbedIOServer("localhost", 9001);

            StatusUpdate("Whats that ticking noise?");
            tickTimer = new Timer { Interval = 1000 / TickRate };
            tickTimer.Elapsed += DoTick;

            Log.Verbose($"Starting LBH with tickrate of {TickRate}tps");
            tickTimer.Start();
            StatusUpdate("Sorting Spaghetti by length");

            InitComplete?.Invoke(this, EventArgs.Empty);
            
        }

        private void PostInit()
        {
            Main = new();
            Main.Show();

            PBConnector = new PickBanConnector();
            ToTick.Add(AppStController);

            Log.Verbose("Checking for running Game");
            AppStController.CheckLeagueRunning();

            PostInitComplete?.Invoke(this, EventArgs.Empty);
        }

        public void OnAppExit()
        {
            //IngameController.OnExit();
        }

        private void DoTick(object sender, EventArgs e)
        {
            //Thread safe iterator
            List<ITickable> tickNow = ToTick.GetRange(0, ToTick.Count);
            tickNow.ForEach(tickable => tickable.DoTick());
        }
    }
}
