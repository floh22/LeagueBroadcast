using System.Collections.Generic;

namespace LeagueBroadcastHub.Events.Game.RiotEvents
{
    class InhibKilled : RiotEvent
    {
        public string TurretKilled;
        public string KillerName;
        public List<string> Assisters;

        public InhibKilled(dynamic e) : base("InhibKilled", (int)e.EventID, (double)e.EventTime)
        {
            this.TurretKilled = e.TurretKilled;
            this.KillerName = e.KillerName;
            this.Assisters = e.Assisters.ToList();
        }
    }
}
