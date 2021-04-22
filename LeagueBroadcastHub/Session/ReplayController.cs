using LeagueBroadcastHub.Data.Provider;
using LeagueBroadcastHub.Data.Replay;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.OperatingSystem;
using LeagueIngameServer;
using System;
using System.Diagnostics;

namespace LeagueBroadcastHub.Session
{
    class ReplayController : ITickable
    {

        public static bool Connected = false;
        public static Process LeagueProcess => GameController.LeagueProcess;

        public static InterfaceState State;

        public ReplayController()
        {
            State = new InterfaceState();
            if(ActiveSettings.current.UseReplayAPI)
            {
                Logging.Verbose("Starting ReplayAPI tick");
                StateController.GameStart += (s, e) => { StartupUI(); Connected = false; };
            }
            StateController.GameStop += (s, e) => { Connected = false; };
        }

        public void DoTick()
        {
        }

        public static void OpenTeamFightUI()
        {
            if (State.TeamfightOpen)
                return;
            InputUtils.SendAToLeague();
            State.TeamfightOpen = true;
        }

        public static void CloseTeamFightUI()
        {
            if (!State.TeamfightOpen)
                return;
            InputUtils.SendAToLeague();
            State.TeamfightOpen = false;
        }

        public static void StartupUI()
        {
            var render = ReplayDataProvider.GETRenderAsync().Result;
            var isDefaultInterface = !render.InterfaceScoreboard && render.InterfaceTimeline;
            if(isDefaultInterface)
            {
                InputUtils.InitLeague();
            }
        }

        private void IsAvailable()
        {

        }

    }
}
