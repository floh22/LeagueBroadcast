using LeagueBroadcastHub.Data.Client.LCU;
using LeagueBroadcastHub.Events.Client;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.Server;
using LeagueBroadcastHub.State.Client;
using LeagueIngameServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static LeagueBroadcastHub.State.Client.StateData;

namespace LeagueBroadcastHub.Session
{
    class ClientController : ITickable
    {
        private static long lastTime = -1;

        //Kinda ugly and not really ported well at all... itll do for now
        private System.Timers.Timer HeartbeatTimer;

        public static bool UpdatedThisTick = false;

        public ClientController()
        {
            StartHeartbeat();
        }

        public async void DoTick()
        {
            //Update the timer since LCUSharp only fires event when champ select changes states
            //Obviously does not include timer update since that would ruin the point of events
            if(!UpdatedThisTick)
            {
                Timer raw = await StateController.GetTimer();
                State.Client.State.data.timer = Converter.ConvertTimer(raw);
                State.Client.State.TriggerUpdate();
            }

            UpdatedThisTick = false;
        }

        public void ApplyNewState(LCUSharp.Websocket.LeagueEvent e)
        {
            Data.Client.LCU.Session tempSession = e.Data.ToObject<Data.Client.LCU.Session>();
            var newState = new CurrentState(true, tempSession);
            if (!State.Client.State.data.champSelectActive)
            {
                Logging.Info("ChampSelect started!");
                State.Client.State.OnChampSelectStarted();
                // Also cache information about summoners
                //Logging.Verbose(e.Data.ToString());
                var t = (Task) StateController.CacheSummoners(newState.session);
                t.Wait();
            }

            lastTime = State.Client.State.data.timer;

            var cleanedData = Converter.ConvertState(newState);
            
            var currentActionBefore = State.Client.State.data.GetCurrentAction();

            State.Client.State.NewState(cleanedData);

            var currentActionAfter = State.Client.State.data.GetCurrentAction();

            if(!currentActionBefore.Equals(currentActionAfter))
            {
                var action = State.Client.State.data.RefreshAction(currentActionBefore);

                State.Client.State.OnNewAction(action);
            }
        }

        public static void OnChampSelectExit(object sender, EventArgs e)
        {
            if (State.Client.State.data.champSelectActive)
            {
                bool finished = State.Client.State.data.timer == 0 && lastTime == 0;
                var finishedText = finished ? "finished" : "ended early";
                Logging.Info($"ChampSelect {finishedText}!");
                State.Client.State.OnChampSelectEnded(finished);
                BroadcastHubController.ToTick.Remove(BroadcastHubController.Instance.clientController);
            }
        }

        public void StartHeartbeat()
        {
            if(HeartbeatTimer == null)
            {
                HeartbeatTimer = new System.Timers.Timer() { Interval = 10000 };
                HeartbeatTimer.Elapsed += (s, e) => {
                    SendHeartbeat();
                };
            } 
            HeartbeatTimer.Enabled = true;
        }

        public void StopHeartbeat()
        {
            HeartbeatTimer.Enabled = false;
        }

        private void SendHeartbeat()
        {
            EmbedIOServer.socketServer.SendEventToAllAsync(new HeartbeatEvent(State.Client.State.GetConfig));
        }
    }
}
