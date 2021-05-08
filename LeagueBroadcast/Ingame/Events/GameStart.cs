using LeagueBroadcast.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Events
{
    public class GameStart : LeagueEvent
    {
        public int EventID;
        public double EventTime;
        public GameStart()
        {
            this.eventType = "GameStart";
            this.EventID = -1;
            this.EventTime = 0;
        }
    }
}
