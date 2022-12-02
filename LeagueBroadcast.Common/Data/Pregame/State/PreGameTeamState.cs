using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Pregame.State
{
    public class PreGameTeamState
    {
        [JsonPropertyName("bans")]
        public List<Ban> Bans { get; set; } = new();
        [JsonPropertyName("picks")]
        public List<Pick> Picks { get; set; } = new();
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not PreGameTeamState or null)
            {
                return false;
            }
            PreGameTeamState other = (obj as PreGameTeamState)!;
            bool sameBans = !Bans.Except(other.Bans).ToList().Any() && !other.Bans.Except(Bans).ToList().Any();
            bool samePicks = !Picks.Except(other.Picks).ToList().Any() && !other.Picks.Except(Picks).ToList().Any();
            bool sameActivity = IsActive == other.IsActive;
            return sameActivity && samePicks && sameBans;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Bans, Picks, IsActive);
        }
    }
}
