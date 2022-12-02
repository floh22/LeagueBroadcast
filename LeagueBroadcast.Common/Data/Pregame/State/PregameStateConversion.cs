using LeagueBroadcast.Common.Data.LCU;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame.State
{
    public class PregameConversionInput
    {
        [JsonPropertyName("team")]
        public List<Cell> Team { get; set; }
        [JsonPropertyName("actions")]
        public List<Common.Data.LCU.Action> Actions { get; set; }

        public PregameConversionInput(List<Cell> team, List<Common.Data.LCU.Action> actions)
        {
            this.Team = team;
            this.Actions = actions;
        }
    }

    public class PregameStateConversionOutput
    {
        [JsonPropertyName("blueTeam")]
        public PreGameTeamState? BlueTeam { get; set; }
        [JsonPropertyName("redTeam")]
        public PreGameTeamState? RedTeam { get; set; }
        [JsonPropertyName("timer")]
        public long Timer { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; } = "";
    }
}
