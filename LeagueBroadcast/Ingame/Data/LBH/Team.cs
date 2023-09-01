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
        public int platesDestroyed;

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
            platesDestroyed = 0;
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
                int teamComponent = id == 0 ? 0 : 5;
                players.ElementAt(i).id = teamComponent + i;
            }
        }

        public float GetGold(int i)
        {
            //Crashing due to CollectionChanged violations
            //return players.Select(p => p.goldHistory.Values.ToList().ElementAtOrDefault(i)).Sum();

            float gold = 0;

            foreach (Player player in players)
            {
                try
                {
                    gold += player.goldHistory.ElementAt(i).Value;
                }
                catch
                {
                    //Ignore for now. One wrong data point rather than crashing
                }

            }

            return gold;
        }

        public float GetGold(double i)
        {
            if (i == lastGoldCalculated)
            {
                return lastGoldValue;
            }

            lastGoldValue = players.Select(p => p.goldHistory.Last().Value).Sum();
            lastGoldCalculated = i;
            return lastGoldValue;
        }
    }
}
