using System.Collections.Generic;

namespace LeagueBroadcastHub.Events.RiotEvents
{
    class ChampionKill : RiotEvent
    {

        public string VictimName;
        public string KillerName;
        public List<string> Assisters;

        public ChampionKill(dynamic e) : base("ChampionKill", (int)e.EventID, (double)e.EventTime)
        {
            this.VictimName = e.VictimName;
            this.KillerName = e.KillerName;
            this.Assisters = e.Assisters.ToList();
        }
    }
}
