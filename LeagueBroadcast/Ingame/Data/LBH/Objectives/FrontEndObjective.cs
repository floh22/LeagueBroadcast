using static LeagueBroadcast.Ingame.Data.LBH.Objectives.Objective;

namespace LeagueBroadcast.Ingame.Data.LBH.Objectives
{
    public class FrontEndObjective
    {
        public string DurationRemaining { get; set; }

        public ObjectiveType Type { get; set; }

        public float GoldDifference { get; set; }

        public double SpawnTimer { get; set; }

        public FrontEndObjective(ObjectiveType Type, double SpawnTimer)
        {
            DurationRemaining = "00:00";
            GoldDifference = 0;
            this.Type = Type;
            this.SpawnTimer = SpawnTimer;
        }
    }
}
