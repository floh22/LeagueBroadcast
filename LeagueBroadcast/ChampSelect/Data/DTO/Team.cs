using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueBroadcast.ChampSelect.Data.DTO
{
    public class Team
    {
        public List<Ban> bans = new List<Ban>();
        public List<Pick> picks = new List<Pick>();
        public bool isActive;

        public override bool Equals(object obj)
        {
            if (!(obj is Team))
            {
                return false;
            }
            var other = obj as Team;
            var sameBans = !bans.Except(other.bans).ToList().Any() && !other.bans.Except(bans).ToList().Any();
            var samePicks = !picks.Except(other.picks).ToList().Any() && !other.picks.Except(picks).ToList().Any();
            var sameActivity = isActive == other.isActive;
            return sameActivity && samePicks && sameBans;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(bans, picks, isActive);
        }
    }
}
