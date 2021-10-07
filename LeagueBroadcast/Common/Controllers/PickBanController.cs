using LeagueBroadcast.ChampSelect.Data.LCU;
using LeagueBroadcast.ChampSelect.Events;
using LeagueBroadcast.ChampSelect.StateInfo;
using LeagueBroadcast.Http;
using LeagueBroadcast.OperatingSystem;
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

        private readonly int maxFailedAttempts = 5;
        private int failedAttempts = 0;

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
            //_ = new State();
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
                if (raw is null)
                {
                    Log.Warn("Tried retrieving pick ban timer while not active. Ignoring");

                    if(failedAttempts++ == maxFailedAttempts)
                    {
                        BroadcastController.Instance.ToTick.Remove(this);
                        failedAttempts = 0;
                    }
                    return;
                }

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
            Log.Info("Starting PickBan Tick");
            Instance.ToTick.Insert(0, this);
            FlagsHelper.Set(ref BroadcastController.CurrentLeagueState, LeagueState.ChampSelect);
        }

        public void OnPickBanExit(object sender, EventArgs e)
        {
            if(State.data.champSelectActive)
            {
                bool finished = State.data.timer == 0 && lastTime == 0;
                var finishedText = finished ? "finished" : "ended early";
                FlagsHelper.Unset(ref BroadcastController.CurrentLeagueState, LeagueState.ChampSelect);
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
            EmbedIOServer.SocketServer?.SendEventToAllAsync(new Heartbeat(State.GetConfig));
        }
    }
}
