using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Common.Tickable;
using LeagueBroadcast.Farsight;
using LeagueBroadcast.Server.Controller;
using LeagueBroadcast.Server.DataProvider;
using LeagueBroadcast.Server.Http;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Server
{
    public class LeagueBroadcastServer : TickController
    {
        private static LeagueBroadcastServer? _instance;

        public static LeagueBroadcastServer Instance
        {
            get
            {
                _instance ??= new LeagueBroadcastServer();
                return _instance;
            }
        }

        private readonly AppConfig _appCfg;


        public PregameController? PregameController { get; private set; }


        private LeagueBroadcastServer()
        {
            TicksPerSecond = 2;
            _appCfg = ConfigController.Get<AppConfig>();
        }

        public void Startup()
        {
            $"Starting LeagueBroadcast Server v{StringVersion.CallingAppVersion}".Info();
            PreInit();
        }


        private void PreInit()
        {
            try
            {
                _ = Logger.RegisterLogger<InstanceFileLogger>();
            }
            catch (InvalidOperationException)
            {
                "Client already registered required loggers".Debug();
            }

            CommunityDragonEventHandler.LoadComplete += (s, e) => Init();
            CommunityDragonDataProvider.Startup();

            BroadcastClientEventHandler.FirePreInitComplete();
        }


        private async void Init()
        {
            LeagueClientEventHandler.ClientConnected += LeagueClientEventHandler_ClientConnected;
            LeagueClientEventHandler.ClientDisconnected += LeagueClientEventHandler_ClientDisconnected;

            await LeagueClientDataProvider.InitConnectionWithinDuration(1000);

            BroadcastClientEventHandler.FireInitComplete();
            LateInit();
        }

        private void LateInit()
        {
            $"Starting Webserver".UpdateStartupProgressText();
            FrontendWebServer.Start("*", _appCfg.WebserverPort);
            BroadcastClientEventHandler.FireLateInitComplete();
        }


        #region EventHandlers

        private void LeagueClientEventHandler_ClientDisconnected(object? sender, EventArgs e)
        {
        }

        private async void LeagueClientEventHandler_ClientConnected(object? sender, EventArgs e)
        {
            LeagueClientEventHandler.ClientConnected -= LeagueClientEventHandler_ClientConnected;
            BroadcastClientEventHandler.ConnectionStatus = ConnectionStatus.Connecting;

            PregameController = PregameController.Init();

            $"Using farsight values for patch {StringVersion.LCUVersion}".Info();
            FarsightConfig cfg = await ConfigController.GetAsync<FarsightConfig>();

            await cfg.Init();

            FarsightDataProvider.Init();
            FarsightDataProvider.ObjectOffsets = cfg.Offsets.GameObject!;
            FarsightDataProvider.GameOffsets = cfg.Offsets.Global!;

            //ChampSelectController.Init();
            //IngameController.Init();

            BroadcastClientEventHandler.ConnectionStatus = ConnectionStatus.Connected;
        }

        #endregion
    }
}
