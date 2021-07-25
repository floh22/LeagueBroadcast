using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.MVVM.ViewModel;
using LeagueBroadcast.OperatingSystem;
using System.Collections.Generic;
using System.Linq;

namespace LeagueBroadcast.Ingame.Data.LBH
{
    public class Team
    {
        public int id;
        public string teamName;
        public string color;

        public List<Player> players;
        public int towers;
        public int kills;

        public bool hasBaron;
        public bool hasElder;

        public int baronTimer = 0;
        public int elderTimer = 0;

        public List<string> dragonsTaken;

        private double lastGoldCalculated = -1;
        private float lastGoldValue = 2500;

        public Team(int teamId, List<Player> players)
        {
            this.id = teamId;
            this.teamName = (id == 0) ? "ORDER" : "CHAOS";
            this.players = players;
            this.color = (id == 0) ? TeamConfigViewModel.BlueTeam.Color.ToSerializedString() : TeamConfigViewModel.RedTeam.Color.ToSerializedString();
            towers = 0;
            hasBaron = false;
            hasElder = false;
            dragonsTaken = new List<string>();
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

        public float GetGold(int i)
        {
            //Get Gold for player at time or 0. Its better than crashing
            return players.Select(p => p.goldHistory.Values.ElementAtOrDefault(i)).Sum();
        }

        public float GetGold(double i)
        {
            if (i == lastGoldCalculated)
                return lastGoldValue;
            lastGoldValue = players.Select(p => p.goldHistory.Values.Last()).Sum();
            lastGoldCalculated = i;
            return lastGoldValue;
        }
    }
}
