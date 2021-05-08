using LeagueBroadcast.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Events
{
    public class RiotEvent : LeagueEvent
    {
        public int EventID { get; set; }
        public string EventName
        {
            get { return eventType; }
            set { eventType = value; }
        }
        public double EventTime { get; set; }
        public List<string> Assisters { get; set; }
        public string KillerName { get; set; }
        public string VictimName { get; set; }
        public string Recipient { get; set; }
#nullable enable
        public string? InhibKilled { get; set; }
        public string? TurretKilled { get; set; }
        public int? KillStreak { get; set; }
#nullable disable
    }

    public static class EventTypes
    {
        public static RiotEvent BARONTAKEN(int GameTime, string Team) => new() { EventID = -1, EventName = "BaronTaken", EventTime = GameTime, Recipient = Team };
        public static RiotEvent DRAGONTAKEN(int GameTime, string Team) => new() { EventID = -1, EventName = "DragonTaken", EventTime = GameTime, Recipient = Team };
        public static RiotEvent BARONEND(int GameTime, string Team) => new() { EventID = -1, EventName = "BaronEnd", EventTime = GameTime, Recipient = Team };
        public static RiotEvent DRAGONEND(int GameTime, string Team) => new() { EventID = -1, EventName = "DragonEnd", EventTime = GameTime, Recipient = Team };
    }
}
