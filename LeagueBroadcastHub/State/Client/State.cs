using LeagueBroadcastHub.Data.Client.DTO;
using LeagueBroadcastHub.Data.Provider;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static LeagueBroadcastHub.State.Client.StateData;

namespace LeagueBroadcastHub.State.Client
{
    class State
    {
        public static StateData data = new StateData();

        public static EventHandler<StateData> StateUpdate;
        public static EventHandler<CurrentAction> NewAction;
        public static EventHandler ChampSelectStarted;
        public static EventHandler<bool> ChampSelectEnded;

        public State()
        {
            Logging.Verbose("StateData config set");
            Logging.Verbose(JsonConvert.SerializeObject(data));
        }

        public static void NewState(Converter.StateConversionOutput state)
        {
            if(!data.blueTeam.Equals(state.blueTeam))
            {
                data.blueTeam = state.blueTeam;
            }
            if(!data.redTeam.Equals(state.redTeam))
            {
                data.redTeam = state.redTeam;
            }
            if(data.timer != state.timer)
            {
                data.timer = state.timer;
            }
            if(data.state != state.state)
            {
                data.state = state.state;
            }

            TriggerUpdate();
        }

        public static void OnChampSelectStarted()
        {
            data.champSelectActive = true;
            ChampSelectStarted?.Invoke(BroadcastHubController.Instance, EventArgs.Empty);
            TriggerUpdate();
        }

        public static void OnChampSelectEnded(bool finished)
        {
            data.champSelectActive = false;
            ChampSelectEnded?.Invoke(BroadcastHubController.Instance, finished);
            TriggerUpdate();
        }

        public static void OnNewAction(CurrentAction action)
        {
            NewAction?.Invoke(BroadcastHubController.Instance, action);
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
            ClientController.UpdatedThisTick = true;
            StateUpdate?.Invoke(BroadcastHubController.Instance, data);
        }

        public static string GetVersionCDN => DataDragon.version.GetVersionCDN();
        public static string GetVersion => DataDragon.version.Champion;
        public static string GetCDN => DataDragon.version.CDN;
        public static Config GetConfig => data.config;
        
    }
}
