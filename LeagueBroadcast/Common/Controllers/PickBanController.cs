using LeagueBroadcast.ChampSelect.Data.LCU;
using LeagueBroadcast.ChampSelect.Events;
using LeagueBroadcast.ChampSelect.State;
using LeagueBroadcast.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Controllers
{
    class PickBanController : ITickable
    {
        public static bool UpdatedThisTick = false;

        private static long lastTime = -1;
        private static System.Timers.Timer HeartbeatTimer;

        public PickBanController()
        {
            if (ConfigController.Component.PickBan.IsActive)
            {

            }
        }

        public void Enable()
        {
            StartHeartbeat();
        }

        public void Disable()
        {
            StopHeartbeat();
        }

        public void DoTick()
        {
            //Update the timer since LCUSharp only fires event when champ select changes states
            //Obviously does not include timer update since that would ruin the point of events
            if (!UpdatedThisTick)
            {
                //Timer raw = await StateController.GetTimer();
                //State.Client.State.data.timer = Converter.ConvertTimer(raw);
                //State.Client.State.TriggerUpdate();
            }

            UpdatedThisTick = false;
        }

        public void StartHeartbeat()
        {
            if (HeartbeatTimer == null)
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
            EmbedIOServer.socketServer.SendEventToAllAsync(new Heartbeat(State.GetConfig));
        }
    }
}
