using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Data.Containers.Objectives
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
