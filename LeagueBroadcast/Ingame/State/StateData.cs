using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.Ingame.Data.LBH.Objectives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.State
{
    public class StateData
    {

        //TODO: Port Controller to v2
        public FrontEndObjective dragon;

        public FrontEndObjective baron;

        public List<string> blueDragons;
        public List<string> redDragons;

        public double gameTime;
        public bool gamePaused;

        public int blueGold;
        public int redGold;

        //public Dictionary<double, int> goldGraph => BroadcastHubController.Instance.gameController.gameState.GetGoldGraph();
        //public List<Inhibitor> inhibitors => BroadcastHubController.Instance.gameController.gameState.GetInhibitors();

        public StateData()
        {
            this.dragon = new FrontEndObjective();
            this.baron = new FrontEndObjective();
            this.blueDragons = new List<string>();
            this.redDragons = new List<string>();

            this.blueGold = 2500;
            this.redGold = 2500;

        }

        public bool ShouldSerializegoldGraph()
        {
            //return IngameController.CurrentSettings.GoldGraph;
            return false;
        }

        public bool ShouldSerializeinhibitors()
        {
            //return IngameController.CurrentSettings.Inhibs;
            return false;
        }

        public bool ShouldSerializebaron()
        {
            //return IngameController.CurrentSettings.SendBaron;
            return false;
        }

        public bool ShouldSerializedragon()
        {
            //return IngameController.CurrentSettings.SendElder;
            return false;
        }
    }
}
