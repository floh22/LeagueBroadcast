using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LeagueBroadcast.Common.Events
{
    public static class LeagueClientEventHandler
    {
        public static event EventHandler? GameStart, GameStop, ChampSelectStart, ChampSelectStop, ClientDisconnected;
        public static event EventHandler<Process>? GameLoad;

        public static event EventHandler<LeagueConnectedEventArgs>? ClientConnected;
        public static event EventHandler? LeagueDisconnected;


        public static int GetClientConnectedInvocationListLength()
        {
            var invocationList = ClientConnected?.GetInvocationList();
            if(invocationList is not null)
            {
                return invocationList.Length;
            }

            return 0;
        }

        public static void FireClientConnected(LeagueConnectedEventArgs args, object? sender = null)
        {
            ClientConnected?.Invoke(sender, args);
        }

        public static void FireClientDisconnected([CallerMemberName] string callerName = "")
        {
            ClientDisconnected?.Invoke(callerName, EventArgs.Empty);
        }


        public static void FireChampSelectStarted([CallerMemberName] string callerName = "")
        {
            ChampSelectStart?.Invoke(callerName, EventArgs.Empty);
        }

        public static void FireChampSelectStopped([CallerMemberName] string callerName = "")
        {
            ChampSelectStop?.Invoke(callerName, EventArgs.Empty);
        }
    }

    public class LeagueConnectedEventArgs : EventArgs
    {
        public long ConnectionDuration { get; set; }

        public LeagueConnectedEventArgs(long connectionDuration)
        {
            ConnectionDuration = connectionDuration;
        }
    }

}
