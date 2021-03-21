using System.Collections.Generic;

namespace LeagueBroadcastHub.Events.RiotEvents
{
    class TurretKilledEvent : RiotEvent
    {
        public string TurretKilled;
        public string KillerName;
        public List<string> Assisters;

        public TurretKilledEvent(dynamic e) : base("TurretKilled", (int)e.EventID, (double)e.EventTime)
        {
            this.TurretKilled = e.TurretKilled;
            this.KillerName = e.KillerName;
            this.Assisters = e.Assisters.ToList();
        }
    }
}
