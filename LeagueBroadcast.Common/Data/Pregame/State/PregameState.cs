using LeagueBroadcast.Common.Events;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame.State
{
    public class PregameState
    {
        [JsonPropertyName("champSelectActive")]
        public bool ChampionSelectActive { get; set; }
        [JsonPropertyName("leagueConnected")]
        public bool LeagueConnected { get; set; }
        [JsonPropertyName("blueTeam")]
        public PreGameTeamState? BlueTeam { get; set; }
        [JsonPropertyName("redTeam")]
        public PreGameTeamState? RedTeam { get; set; }
        [JsonPropertyName("timer")]
        public long Timer { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("config")]
        public PreGameDisplayData Config { get; set; }


        public PregameState(bool championSelectActive, bool leagueConnected, PreGameTeamState blueTeam, PreGameTeamState redTeam, long timer, string state, PreGameDisplayData config)
        {
            ChampionSelectActive = championSelectActive;
            LeagueConnected = leagueConnected;
            BlueTeam = blueTeam;
            RedTeam = redTeam;
            Timer = timer;
            State = state;
            Config = config;
        }

        public PregameState(PreGameDisplayData config)
        {
            ChampionSelectActive = false;
            LeagueConnected = false;
            Timer = 0;
            State = "Waiting";
            Config = config;
        }

        public CurrentAction GetCurrentAction()
        {

            if ((BlueTeam is null || RedTeam is null)
                || (BlueTeam.IsActive && RedTeam.IsActive)
                || (!BlueTeam.IsActive && !RedTeam.IsActive))
            {
                return new CurrentAction() { State = "none" };
            }

            PreGameTeamState activeTeam = BlueTeam.IsActive ? BlueTeam : RedTeam;
            string activeTeamName = BlueTeam.IsActive ? "blueTeam" : "redTeam";

            List<Ban> activeBans = activeTeam.Bans.Where(ban => ban.IsActive).ToList();
            List<Pick> activePicks = activeTeam.Picks.Where(pick => pick.IsActive).ToList();

            if (activeBans.Count > 0)
            {
                return new CurrentAction()
                {
                    State = "ban",
                    Data = new List<PickBan>() { activeBans.ElementAt(0) },
                    Team = activeTeamName,
                    Num = activeTeam.Bans.IndexOf(activeBans.ElementAt(0))
                };

            }

            if (activePicks.Count > 0)
            {
                return new CurrentAction()
                {
                    State = "pick",
                    Data = new List<PickBan>() { activePicks.ElementAt(0) },
                    Team = activeTeamName,
                    Num = activeTeam.Picks.IndexOf(activePicks.ElementAt(0))
                };
            }

            return new CurrentAction() { State = "none" };
        }

        public CurrentAction RefreshAction(CurrentAction action)
        {
            if (action.State == "none")
            {
                return action;
            }

            var team = action.Team == "blueTeam" ? BlueTeam : RedTeam;

            action.Data.Clear();
            //Do not refresh if champ select has ended
            if (team is null || (team.Picks.Count == 0 && team.Bans.Count == 0))
            {
                return action;
            }

            if (action.State == "ban")
            {
                action.Data.Add(team.Bans.ElementAt(action.Num));
            }
            else
            {
                action.Data.Add(team.Picks.ElementAt(action.Num));
            }

            return action;
        }


        public void NewState(PregameStateConversionOutput conversionOutput)
        {
            BlueTeam = conversionOutput.BlueTeam;
            RedTeam = conversionOutput.RedTeam;
            Timer = conversionOutput.Timer;
            State = conversionOutput.State;

            PregameControllerEventHandler.FireStateUpdate(new StateUpdateEventArgs(this), null);
        }
    }
}
