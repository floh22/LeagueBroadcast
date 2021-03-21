using System;

namespace LeagueBroadcastHub.Data.Containers.Objectives
{
    public class Objective
    {
        public string Type { get; set; }
        public int Cooldown { get; set; }
        public bool IsAlive { get; set; }
        public int TimesTakenInMatch { get; set; }
        public int LastTakenBy { get; set; }
        public bool FoundTeam { get; set; }
        public double TimeSinceTaken { get; set; }

        public Objective()
        {
            this.Type = "";
            this.Cooldown = int.MaxValue;
            this.IsAlive = false;
            this.TimesTakenInMatch = 0;
            this.LastTakenBy = -1;
            this.FoundTeam = false;
            this.TimeSinceTaken = 0;
        }

        public ObjectiveType GetObjectiveType()
        {
            if (Type.Equals("Baron", StringComparison.OrdinalIgnoreCase))
                return ObjectiveType.Baron;
            return ObjectiveType.Dragon;
        }

        public enum ObjectiveType
        {
            Baron,
            Dragon
        }
    }
}
