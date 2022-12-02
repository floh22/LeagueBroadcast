using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Events
{
    public static class BroadcastClientEventHandler
    {
        public static event EventHandler<ConnectionStatus>? ClientConnectionStatusUpdate;
        public static event EventHandler<StartupProgressUpdateEventArgs>? StartupProgressUpdate;
        public static event EventHandler<string>? StartupProgressTextUpdate;

        public static event EventHandler? PreInitComplete, InitComplete, LateInitComplete;

        private static ConnectionStatus _connectionStatus;
        public static ConnectionStatus ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                _connectionStatus = value;
                ClientConnectionStatusUpdate?.Invoke(null, value);
            }
        }


        static BroadcastClientEventHandler()
        {
            LeagueClientEventHandler.GameStart += LeagueClientEventHandler_GameStart;
        }

        private static void LeagueClientEventHandler_GameStart(object? sender, EventArgs e)
        {
            ConnectionStatus = ConnectionStatus.Ingame;
        }

        public static void UpdateStartupProgressText(this string text)
        {
            StartupProgressTextUpdate?.Invoke(null, text);
        }

        public static void FirePreInitComplete([CallerMemberName] string callerName = "")
        {
            PreInitComplete?.Invoke(callerName, EventArgs.Empty);
        }

        public static void FireInitComplete([CallerMemberName] string callerName = "")
        {
            InitComplete?.Invoke(callerName, EventArgs.Empty);
        }

        public static void FireLateInitComplete([CallerMemberName] string callerName = "")
        {
            LateInitComplete?.Invoke(callerName, EventArgs.Empty);
        }
    }

    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        PreGame,
        Ingame,
        PostGame
    }

    public enum LoadStatus
    {
        PreInit = 3,
        CDragon = 5,
        Init = 80,
        PostInit = 95,
        FinishInit = 100
    }

    public class StartupProgressUpdateEventArgs : EventArgs
    {
        public LoadStatus Status { get; set; }
        public int Progress { get; set; }
    }
}
