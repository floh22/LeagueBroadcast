using LeagueBroadcast.Ingame.Data.RIOT;
using System.Collections.Generic;
using System.Linq;

namespace LeagueBroadcast.Ingame.Data.LBH
{
    public class Team
    {
        public int id;
        public string teamName;

        public List<Player> players;
        public int gold;
        public int towers;

        public bool hasBaron;
        public bool hasElder;

        public int baronTimer = 0;
        public int elderTimer = 0;

        public List<string> dragonsTaken;
        public Dictionary<double, int> goldHistory;

        public Team(int teamId, List<Player> players)
        {
            this.id = teamId;
            this.teamName = (id == 0) ? "ORDER" : "CHAOS";
            this.players = players;
            gold = 2500;
            towers = 0;
            hasBaron = false;
            hasElder = false;
            dragonsTaken = new List<string>();
            goldHistory = new Dictionary<double, int>();
        }

        public int GetDragonsTaken()
        {
            return dragonsTaken.Count;
        }

        public void UpdateIDs()
        {
            for (int i = 0; i < players.Count; i++)
            {
                var teamComponent = id == 0 ? 0 : 5;
                players.ElementAt(i).id = teamComponent + i;
            }
        }
    }
}
