using LeagueBroadcast.ChampSelect.Data.Config;
using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.OperatingSystem;
using Newtonsoft.Json;
using System;
using System.Linq;
using static LeagueBroadcast.ChampSelect.StateInfo.StateData;

namespace LeagueBroadcast.ChampSelect.StateInfo
{
    class State
    {
        public static StateData data = new ();

        public static EventHandler<StateData> StateUpdate;
        public static EventHandler<CurrentAction> NewAction;
        public static EventHandler ChampSelectStarted;
        public static EventHandler<bool> ChampSelectEnded;

        public State()
        {
            Log.Info("StateData config set");
            Log.Info(JsonConvert.SerializeObject(data));
        }

        public static void NewState(Converter.StateConversionOutput state)
        {
            if (!data.blueTeam.Equals(state.blueTeam))
            {
                data.blueTeam = state.blueTeam;
            }
            if (!data.redTeam.Equals(state.redTeam))
            {
                data.redTeam = state.redTeam;
            }
            if (data.timer != state.timer)
            {
                data.timer = state.timer;
            }
            if (data.state != state.state)
            {
                data.state = state.state;
            }

            TriggerUpdate();
        }

        public static void OnChampSelectStarted()
        {
            data.champSelectActive = true;
            ChampSelectStarted?.Invoke(BroadcastController.Instance, EventArgs.Empty);
            TriggerUpdate();
        }

        public static void OnChampSelectEnded(bool finished)
        {
            data.champSelectActive = false;
            ChampSelectEnded?.Invoke(BroadcastController.Instance, finished);
            TriggerUpdate();
        }

        public static void OnNewAction(CurrentAction action)
        {
            NewAction?.Invoke(BroadcastController.Instance, action);
        }

        public static void LeagueConntected()
        {
            data.leagueConnected = true;
            TriggerUpdate();
        }

        public static void LeagueDisconnected()
        {
            data.leagueConnected = false;
            TriggerUpdate();
        }

        public static void TriggerUpdate()
        {
            PickBanController.UpdatedThisTick = true;
            StateUpdate.Invoke(BroadcastController.Instance, data);
        }

        public static string GetVersionCDN => DataDragon.version.GetVersionCDN();
        public static string GetVersion => DataDragon.version.Champion;
        public static string GetCDN => DataDragon.version.CDN;
        public static PickBanConfig GetConfig => data.config;

    }
}
