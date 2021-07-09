using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.Http;
using LeagueBroadcast.MVVM.View;
using LeagueBroadcast.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using LeagueBroadcast.Farsight;
using static LeagueBroadcast.Common.Log;
using LeagueBroadcast.Ingame.Data.Provider;

namespace LeagueBroadcast.Common.Controllers
{
    class BroadcastController
    {
        public static string AppVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public static int TickRate = 2;
        public static LeagueState CurrentLeagueState;
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
        public FarsightController MemoryController;
        public GameInputController GIController;

        public List<ITickable> ToTick;

        public MainWindow Main { get; private set; }
        public StartupWindow Startup { get; private set; }


        private Timer tickTimer;
        private StartupViewModel _startupContext;
        private MainViewModel _mainContext;

        private DateTime loadStart, initFinish;
        private BroadcastController()
        {
            loadStart = DateTime.Now;
            initFinish = DateTime.Now;

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
            _startupContext.Status = Status;
        }

        private async void EarlyInit()
        {
            DataDragon.FinishLoading += (s, e) => Init();
            InitComplete += (s, e) => PostInit();

            Startup = new StartupWindow();
            Startup.Show();
            _startupContext = (StartupViewModel)Startup.DataContext;
            _startupContext.Status = "Early Init";

            ToTick = new();
            
            _ = new Log(LogLevel.Verbose, FileVersionInfo.GetVersionInfo("LeagueBroadcast.exe").FileVersion);
            
            
            CfgController = ConfigController.Instance;
            Log.SetLogLevel(ConfigController.Component.App.LogLevel);
            Log.Info($"League Broadcast Version {ConfigController.Component.App.Version}");
            if (await AppUpdateController.Update(_startupContext)) {
                Startup.Close();
                App.Instance.Shutdown();
            } else
            {
                Log.Info("Using current Version");
            }

            EarlyInitComplete?.Invoke(null, EventArgs.Empty);
            Log.Info($"Early Init Complete in {(DateTime.Now - loadStart).TotalMilliseconds}ms");
            initFinish = DateTime.Now;

            _startupContext.UpdateLoadProgress(LoadStatus.PreInit);

            await Task.Delay(50);

            DDragon = DataDragon.Instance;
        }

        private void Init()
        {
            Log.Info("DDragon loaded");

            StatusUpdate("Loading PickBan Controller");
            PBController = new();
            _startupContext.UpdateLoadProgress(LoadStatus.Init, 25);

            StatusUpdate("Loading Ingame Controller");
            IGController = new();
            GIController = new();
            _startupContext.UpdateLoadProgress(LoadStatus.Init, 35);

            StatusUpdate("Loading Replay Controller");
            ReplayController = new();
            _startupContext.UpdateLoadProgress(LoadStatus.Init, 50);

            StatusUpdate("Loading Farsight");
            MemoryController = new();

            StatusUpdate("Loading Frontend Webserver (HTTP/WS)");
            var WebServer = new EmbedIOServer("*", 9001);
            _startupContext.UpdateLoadProgress(LoadStatus.Init, 85);

            StatusUpdate("Whats that ticking noise?");
            tickTimer = new Timer { Interval = 1000 / TickRate };
            tickTimer.Elapsed += DoTick;
            _startupContext.UpdateLoadProgress(LoadStatus.Init);

            Log.Info($"Init Complete in {(DateTime.Now - initFinish).ToString(@"s\.fff")}s");
            initFinish = DateTime.Now;
            InitComplete?.Invoke(null, EventArgs.Empty);
            

        }

        private void PostInit()
        {
            Log.Info("Opening main window");
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Main = new();
                Main.Show();

                _mainContext = (MainViewModel)Main.DataContext;
                _mainContext.SetMainWindow(Main);

                StatusUpdate("Loading State Controller");
                AppStController = AppStateController.Instance;
            });

            AppStController.Init();

            AppStateController.GameStart += (s, p) => {
                MemoryController.Connect(p);
                
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    _mainContext.ConnectionStatus = ConnectionStatusViewModel.CONNECTED;
                });
                
            };

            AppStateController.GameStop += (s, e) => {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    _mainContext.ConnectionStatus = ConnectionStatusViewModel.LCU;
                });
            };



            _startupContext.UpdateLoadProgress(LoadStatus.PostInit, 33);

            PBConnector = new PickBanConnector();
            ToTick.Add(AppStController);
            _startupContext.UpdateLoadProgress(LoadStatus.PostInit, 66);

            Application.Current.Dispatcher.Invoke((Action)delegate {
                Startup.Close();
            });

            Log.Info($"Starting LeagueBroadcast with tickrate of {TickRate}tps");
            tickTimer.Start();

            Log.Info("Checking for running Game");
            IGController.StartWaitingForTargetProcess();
            _startupContext.UpdateLoadProgress(LoadStatus.PostInit);

            PostInitComplete?.Invoke(null, EventArgs.Empty);
            Log.Info($"Post Init Complete in {(DateTime.Now - initFinish).TotalMilliseconds}ms");
            Log.Info($"Total Startup time: {DateTime.Now - loadStart:s\\.fff}s");
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

    [Flags]
    public enum LeagueState
    {
        Connected,
        ChampSelect,
        InProgress,
        PostGame
    }
}
