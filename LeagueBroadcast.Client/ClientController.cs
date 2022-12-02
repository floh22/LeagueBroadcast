using LeagueBroadcast.Client.MVVM.View.Startup;
using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Common.Exceptions;
using LeagueBroadcast.Update;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using LeagueBroadcast.Server;
using System;
using System.IO;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;
using System.Windows.Forms;
using System.Drawing;
using LeagueBroadcast.Client.MVVM.ViewModel.Startup;
using LeagueBroadcast.Client.Services;
using System.Reflection;
using System.Diagnostics;

namespace LeagueBroadcast.Client
{
    internal class ClientController
    {
        private static ClientController? _instance;
        public static ClientController Instance
        {
            get
            {
                _instance ??= new ClientController();
                return _instance;
            }
        }

        public StartupWindow? StartupWin { get; private set; }

        public StartupViewModel? StartupCtx { get; private set; }

        private readonly AppConfig appConfig;
        private readonly ClientConfig clientConfig;

        private LeagueBroadcastServer? _server;

        private readonly NotifyIcon _notifyIcon;

        public static async Task<ClientController> BuildClientControllerAsync()
        {
            ClientController controller = new();

            $"Client Controller created".Debug();

            //Init logging
            _ = Logger.RegisterLogger<InstanceFileLogger>();

            //Create Startup window
            controller.StartupWin = new StartupWindow();
            controller.StartupWin.Show();
            controller.StartupCtx = (StartupViewModel)controller.StartupWin.DataContext;

            //Check for updates
            if (await UpdateController.CheckForUpdate("LeagueBroadcast", controller.clientConfig.LastSkippedVersion))
            {
                //No point in continuing startup if we are going to restart in a moment anyway
                UpdateController.UpdateDownloaded += (s, e) =>
                {
                    UpdateController.RestartApplication();
                };
                return controller;
            }

            BroadcastClientEventHandler.PreInitComplete += controller.OnPreInit;
            BroadcastClientEventHandler.InitComplete += controller.OnInit;
            BroadcastClientEventHandler.LateInitComplete += controller.OnLateInit;

            $"Starting League Broadcast Server".Debug();
            controller._server = LeagueBroadcastServer.Instance;
            controller._server.Startup();
            return controller;

        }

        private ClientController()
        {
            $"Starting LeagueBroadcast Client v{StringVersion.CallingAppVersion}".Info();


            BroadcastClientEventHandler.StartupProgressTextUpdate += StatusUpdate;
            BroadcastClientEventHandler.StartupProgressUpdate += ProgressUpdate;

            WorkingDirectory.SetDirectory(Directory.GetCurrentDirectory());

            //Load app and client configs before anything else since this determines how to proceed with loading
            if (!(ConfigController.RegisterConfig<AppConfig>() && ConfigController.RegisterConfig<ClientConfig>()))
            {
                //Something went very wrong...
                throw new InvalidConfigException("Could not load main config file. App cannot run");
            }
            appConfig = ConfigController.Get<AppConfig>();
            clientConfig = ConfigController.Get<ClientConfig>();


            _notifyIcon = new Forms.NotifyIcon();

            App.Current.Exit += OnExit;
        }

        private void OnPreInit(object? sender, EventArgs e)
        {
            string[] res = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string resName in res)
                resName.Debug();

            Stream? blueEssenceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LeagueBroadcast.Client.Assets.Icons.BlueEssence.ico");
            if (blueEssenceStream is not null)
            {
                _notifyIcon.Icon = new Icon(blueEssenceStream);
                _notifyIcon.Visible = true;
                _notifyIcon.Text = "LeagueBroadcast";

                _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
                _notifyIcon.ContextMenuStrip.Items.Add("Hotkeys", Image.FromStream(blueEssenceStream), OnHotkeysClicked);
                _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, OnExitClicked);
            }
        }

        private void OnInit(object? sender, EventArgs e)
        {
            FrontendWebServerEventHandler.WebServerReady += FrontendWebServerEventHandler_WebServerReady;
        }

        private void FrontendWebServerEventHandler_WebServerReady(object? sender, EventArgs e)
        {
            $"Opening Webbrowser".Info();
            _ = Process.Start("explorer.exe", $"http://localhost:{appConfig.WebserverPort}/interface");

            StartupWin?.Hide();
        }

        private void OnLateInit(object? sender, EventArgs e)
        {
            $"League Broadcast started".Info("Client");
        }


        private void OnExit(object? sender, EventArgs e)
        {
            _notifyIcon.Dispose();
        }


        private void StatusUpdate(object? sender, string status)
        {
            if (StartupCtx is not null)
                StartupCtx.StatusMessage = status;
        }

        private void ProgressUpdate(object? sender, StartupProgressUpdateEventArgs e)
        {
            StartupCtx?.UpdateLoadProgress(e.Status, e.Progress);
        }


        private void OnHotkeysClicked(object? sender, EventArgs e)
        {

        }

        private void OnExitClicked(object? sender, EventArgs e)
        {
            $"Exiting League Broadcast".Info();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
