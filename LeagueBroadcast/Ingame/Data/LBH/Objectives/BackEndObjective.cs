namespace LeagueBroadcast.Ingame.Data.LBH.Objectives
{
    class BackEndObjective
    {
        public int BlueStartGold;
        public int RedStartGold;

        public double DurationRemaining;

        public BackEndObjective()
        {
            this.BlueStartGold = 0;
            this.RedStartGold = 0;
            this.DurationRemaining = -1;
        }
    }
}
