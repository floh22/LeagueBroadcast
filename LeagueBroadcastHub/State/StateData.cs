using System.Collections.Generic;
using LeagueBroadcastHub.Data.Containers.Objectives;

namespace LeagueBroadcastHub.State
{
    class StateData
    {
        public FrontEndObjective dragon;

        public FrontEndObjective baron;

        public List<string> blueDragons;
        public List<string> redDragons;

        public double gameTime;
        public bool gamePaused;

        public int blueGold;
        public int redGold;

        public StateData()
        {
            this.dragon = new FrontEndObjective();
            this.baron = new FrontEndObjective();
            this.blueDragons = new List<string>();
            this.redDragons = new List<string>();

            this.blueGold = 2500;
            this.redGold = 2500;

        }
    }
}
