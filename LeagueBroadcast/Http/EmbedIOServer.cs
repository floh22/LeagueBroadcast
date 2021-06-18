using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using LeagueBroadcast.Common;
using LeagueBroadcast.OperatingSystem;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LeagueBroadcast.Http
{
    class EmbedIOServer
    {
        private WebServer webServer;

        public static WSServer SocketServer;

        public EmbedIOServer(string location, int port)
        {
            var uri = $"http://{location}:{port}/";

            webServer = CreateWebServer(uri);

            webServer.RunAsync();
            Log.Info($"WebServer running on {uri}");
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            webServer.Dispose();
            Log.Info($"WebServer stopped");
        }

        private static WebServer CreateWebServer(string url)
        {
            var webRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            Log.Info($"Server file system starting");
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // Add modules
                .WithLocalSessionManager()
                .WithCors()
                .WithModule(SocketServer = new("/api"))
                .WithModule(new FileModule("/cache",
                    new FileSystemProvider(webRoot, false))
                {
                    DirectoryLister = DirectoryLister.Html
                })
                // Static files last to avoid conflicts
                .WithStaticFolder("/frontend", $"{Directory.GetCurrentDirectory()}\\Frontend\\ingame", true, m => m
                    .WithContentCaching(true))
                .WithStaticFolder("/", $"{Directory.GetCurrentDirectory()}\\Frontend\\pickban", true, m => m
                    .WithContentCaching(true))
                ;

            // Listen for state changes.
            server.StateChanged += (s, e) => Log.Info($"WebServer New State - {e.NewState}");

            return server;
        }
    }
}
