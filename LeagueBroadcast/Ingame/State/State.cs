using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.Ingame.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.State
{
    class State
    {
        private IngameController controller;

        public StateData stateData;
        public List<RiotEvent> pastIngameEvents;
        public int lastGoldDifference;
        public double lastGoldUpdate;

        public Team blueTeam;
        public Team redTeam;

        public State(IngameController controller)
        {
            pastIngameEvents = new List<RiotEvent>();
            this.stateData = null;
            this.controller = controller;
            this.blueTeam = null;
            this.redTeam = null;
            this.lastGoldDifference = 0;
            this.lastGoldUpdate = 0;
            AppStateController.GameStop += (s, e) => { lastGoldDifference = 0; lastGoldUpdate = 0; };
        }
    }
}
