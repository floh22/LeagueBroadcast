using EmbedIO;
using EmbedIO.Files;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Server.Http
{
    internal static class FrontendWebServer
    {
        private static WebServer? _webServer;

        private static WebSocketServer? _preGameServer, _ingameServer, _postGameServer, _userInterfaceServer;

        public static WebSocketServer PreGameServer
        {
            get
            {
                if(_preGameServer is null)
                {
                    _preGameServer = new WebSocketServer("pregameSocket");
                    _preGameServer.ClientConnected += (s, context) => {
                        //_pregameServer.SendEventAsync(context, new PreGame.ChampSelect.Events.NewState(PreGame.ChampSelect.StateInfo.State.data));
                    };
                }

                return _preGameServer;
            }
        }


        public static WebSocketServer InGameServer
        {
            get
            {
                if (_ingameServer is null)
                {
                    _ingameServer = new WebSocketServer("ingameSocket");
                    _ingameServer.ClientConnected += (s, context) => {

                    };
                }

                return _ingameServer;
            }
        }

        public static WebSocketServer PostGameServer
        {
            get
            {
                if (_postGameServer is null)
                {
                    _postGameServer = new WebSocketServer("postgameSocket");
                    _postGameServer.ClientConnected += (s, context) => {

                    };
                }

                return _postGameServer;
            }
        }

        public static WebSocketServer UserInterfaceServer
        {
            get
            {
                if (_userInterfaceServer is null)
                {
                    _userInterfaceServer = new WebSocketServer("interfaceSocket");
                    _userInterfaceServer.ClientConnected += (s, context) => {

                    };
                }

                return _userInterfaceServer;
            }
        }


        public static void Start(string location, int port)
        {
            var uri = $"http://{location}:{port}/";

            _webServer = CreateWebServer(uri);


            _webServer.RunAsync();
            $"WebServer running on {uri}".Info("FrontendWebServer");
        }

        public static void Restart()
        {
            throw new NotImplementedException();
        }

        public static void Stop()
        {
            _webServer?.Dispose();
            $"WebServer stopped".Info("FrontendWebServer");
        }

        private static WebServer CreateWebServer(string url)
        {
            var webRoot = Path.Combine(WorkingDirectory.GetDirectory(), "Data", "Cache");
            $"Server file system starting".Info("EmbedIO");
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // Add modules
                .WithLocalSessionManager()
                .WithCors()
                .WithModule(PreGameServer)
                .WithModule(InGameServer)
                .WithModule(PostGameServer)
                .WithModule(UserInterfaceServer)
                .WithModule(new FileModule("/cache",
                    new FileSystemProvider(webRoot, false))
                {
                    DirectoryLister = DirectoryLister.Html
                })
                // Static files last to avoid conflicts
                .WithStaticFolder("/ingame", $"{WorkingDirectory.GetDirectory()}\\Frontend\\ingame", true, m => m
                    .WithContentCaching(true))
                .WithStaticFolder("/pregame", $"{WorkingDirectory.GetDirectory()}\\Frontend\\pregame", true, m => m
                    .WithContentCaching(true))
                .WithStaticFolder("/postgame", $"{WorkingDirectory.GetDirectory()}\\Frontend\\postgame", true, m => m
                    .WithContentCaching(true))
                .WithStaticFolder("/interface", $"{WorkingDirectory.GetDirectory()}\\Frontend\\userinterface", true, m => m
                    .WithContentCaching(true))
                ;


            string dir = $"{WorkingDirectory.GetDirectory()}\\Frontend\\userinterface";

            // Listen for state changes.
            server.StateChanged += OnWebServerStateChanged;

            return server;
        }


        private static void OnWebServerStateChanged(object? sender, WebServerStateChangedEventArgs e)
        {
            $"WebServer New State - {e.NewState}".Debug("FrontendWebServer");
            if(e.NewState == WebServerState.Listening)
            {
                $"Web Server ready".Info("FrontendWebServer");
                FrontendWebServerEventHandler.FireWebServerReady();
            }
        }
    }
}
