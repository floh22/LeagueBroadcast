using LCUSharp;
using LCUSharp.Websocket;
using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Common.Data.LCU;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Server.Controller;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Text.Json;

namespace LeagueBroadcast.Server.DataProvider
{
    public static class LeagueClientDataProvider
    {


        private static readonly ComponentConfig _componentConfig;

        private static LeagueClientApi? _leagueClientApi { get; set; }

        private static Task? _initTask;


        static LeagueClientDataProvider()
        {
            _componentConfig = ConfigController.Get<ComponentConfig>();
        }

        private static async void LeagueClientApi_Disconnected(object? sender, EventArgs e)
        {
            "Client Disconnected! Attempting to reconnect...".Info();
            BroadcastClientEventHandler.ConnectionStatus = ConnectionStatus.Disconnected;
            LeagueClientEventHandler.FireClientDisconnected();
            var reconnectTask =  _leagueClientApi?.ReconnectAsync();
            if(reconnectTask is not null)
            {
                await reconnectTask;
            } else
            {
                $"Client API unable to reconnect. League Broadcast functions related to the LCU will not work".Error();
                return;
            }
            "Client Reconnected!".Info();
            LeagueClientEventHandler.FireClientConnected(new LeagueConnectedEventArgs(0));
        }


        public static async Task<bool> InitConnectionWithinDuration(int timeoutInMS)
        {
            "Starting League Client connection".UpdateStartupProgressText();
            _initTask = InitClientConenction();
            return await Task.WhenAny(_initTask, Task.Delay(timeoutInMS)) == _initTask;
        }

        private static async Task InitClientConenction()
        {
            "Connecting to League Client".Info();
            Stopwatch stopwatch = Stopwatch.StartNew();
            _leagueClientApi = await LeagueClientApi.ConnectAsync();
            _leagueClientApi.EventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", LeagueClientApi_ClientStateChanged);
            _leagueClientApi.EventHandler.Subscribe("/lol-champ-select/v1/session", LeagueClientApi_ChampSelectStateChanged);
            _leagueClientApi.Disconnected += LeagueClientApi_Disconnected;
            stopwatch.Stop();
            $"Connected to League Client in {stopwatch.ElapsedMilliseconds} ms".Info();

            if (LeagueClientEventHandler.GetClientConnectedInvocationListLength() == 0)
            {
                $"Something went wrong".Info();
            }

            await LeagueClientDataProvider.GetLocalGameVersion();

            LeagueClientEventHandler.FireClientConnected(new LeagueConnectedEventArgs(stopwatch.ElapsedMilliseconds));
        }

        private static void LeagueClientApi_ClientStateChanged(object? caller, LeagueEvent e)
        {
            string eventType = e.Data.ToString();
            $"League State: {eventType}".Info();


            if (!eventType.Equals("ChampSelect", StringComparison.Ordinal) && BroadcastClientEventHandler.ConnectionStatus == ConnectionStatus.PreGame)
            {
                LeagueClientEventHandler.FireChampSelectStopped();
            }

            if (eventType.Equals("ChampSelect", StringComparison.Ordinal) && BroadcastClientEventHandler.ConnectionStatus != ConnectionStatus.PreGame)
            {
                LeagueClientEventHandler.FireChampSelectStarted();
            }
        }

        private static void LeagueClientApi_ChampSelectStateChanged(object? caller, LeagueEvent e)
        {
            if(_componentConfig.PickBan.IsActive && BroadcastClientEventHandler.ConnectionStatus == ConnectionStatus.PreGame)
            {
                PregameController.Instance.ApplyNewState(e.Data.ToObject<Session>());
            }
        }

        public static async Task GetLocalGameVersion()
        {
            $"Determining local game version".Info();

            if (_leagueClientApi is null)
            {
                $"Client not connected".Error();
                return;
            }
            string? gameVersion = null;

            while (gameVersion is null || gameVersion == "")
            {
                try
                {
                    gameVersion = await _leagueClientApi!.RequestHandler.GetResponseAsync<string>(HttpMethod.Get, "/lol-patch/v1/game-version");
                }
                catch (Exception e)
                {
                    e.Message.Error();
                    // Ignored
                }
                await Task.Delay(200);
            }
            $"Local client running version {gameVersion}".Debug();

            string[] patchComponents = gameVersion.Split(".");
            StringVersion.SetLCUClientVersion( new(int.Parse(patchComponents[0]),
                                   int.Parse(patchComponents[1]),
                                   1));
        }

        public static Dictionary<Cell, Task<string>> GetPlayersInTeam(List<Cell> team)
        {
            Dictionary<Cell, Task<string>> toFinish = new();
            team.ForEach(cell => {
                if (cell.SummonerId == "0")
                {
                    return;
                }

                try
                {
                    toFinish.Add(cell, _leagueClientApi!.RequestHandler.GetJsonResponseAsync(HttpMethod.Get, $"lol-summoner/v1/summoners/{cell.SummonerId}"));
                }
                catch (Exception e)
                {
                    "Could not fetch players for team. Is this not a custom game?".Error();
                    e.Message.Error();
                }

            });

            return toFinish;
        }

        public static async Task<SessionTimer?> GetChampSelectSessionTimer()
        {
            try
            {
                return JsonSerializer.Deserialize<SessionTimer>(await _leagueClientApi!.RequestHandler.GetJsonResponseAsync(HttpMethod.Get, $"/lol-champ-select/v1/session/timer"));
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public static async Task<Session> GetChampSelectSession()
        {
            return await _leagueClientApi!.RequestHandler.GetResponseAsync<Session>(HttpMethod.Get, $"/lol-champ-select/v1/session");
        }
    }
}
