using LeagueBroadcast.ChampSelect.Data.LCU;
using LeagueBroadcast.ChampSelect.Events;
using LeagueBroadcast.ChampSelect.StateInfo;
using LeagueBroadcast.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
                Enable();
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

        public async void DoTick()
        {
            //Update the timer since LCUSharp only fires event when champ select changes states
            //Obviously does not include timer update since that would ruin the point of events
            if (!UpdatedThisTick)
            {
                Timer raw = await AppStateController.GetTimer();
                State.data.timer = Converter.ConvertTimer(raw);
                State.TriggerUpdate();
            }

            UpdatedThisTick = false;
        }
        public void ApplyNewState(LCUSharp.Websocket.LeagueEvent e)
        {
            Session tempSession = e.Data.ToObject<Session>();
            var newState = new CurrentState(true, tempSession);
            if (!State.data.champSelectActive)
            {
                Log.Info("ChampSelect started!");
                State.OnChampSelectStarted();
                // Also cache information about summoners since this wont change
                var t = (Task)AppStateController.CacheSummoners(newState.session);
                t.Wait();
            }

            lastTime = State.data.timer;

            var cleanedData = Converter.ConvertState(newState);

            var currentActionBefore = State.data.GetCurrentAction();

            State.NewState(cleanedData);

            var currentActionAfter = State.data.GetCurrentAction();

            if (!currentActionBefore.Equals(currentActionAfter))
            {
                var action = State.data.RefreshAction(currentActionBefore);

                State.OnNewAction(action);
            }
        }

        public void OnEnterPickBan(object sender, EventArgs e)
        {
            var Instance = BroadcastController.Instance;
            if (Instance.ToTick.Contains(this))
            {
                return;
            }

            Log.Info("Starting PickBan Controller");
            Instance.ToTick.Insert(0, this);

            if (BroadcastController.CurrentLeagueState == "InProgress")
            {
                Instance.ToTick.Remove(Instance.IGController);
            }
            BroadcastController.CurrentLeagueState = "ChampSelect";
        }

        public void OnPickBanExit(object sender, EventArgs e)
        {
            if(State.data.champSelectActive)
            {
                bool finished = State.data.timer == 0 && lastTime == 0;
                var finishedText = finished ? "finished" : "ended early";
                Log.Info($"ChampSelect {finishedText}!");
                State.OnChampSelectEnded(finished);
                BroadcastController.Instance.ToTick.Remove(this);
            }
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
