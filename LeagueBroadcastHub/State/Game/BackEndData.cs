using LeagueBroadcastHub.Data.Game.Containers.Objectives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.State.Game
{
    class BackEndData
    {
        public BackEndObjective dragon;

        public BackEndObjective baron;

        public BackEndData()
        {
            this.dragon = new BackEndObjective();
            this.baron = new BackEndObjective();
        }
    }
}
