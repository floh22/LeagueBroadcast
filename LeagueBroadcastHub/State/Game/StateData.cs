using LeagueBroadcastHub.Data.Game.Containers;
using LeagueBroadcastHub.Data.Game.Containers.Objectives;
using LeagueBroadcastHub.Log;
using LeagueBroadcastHub.Session;
using LeagueIngameServer;
using System.Collections.Generic;

namespace LeagueBroadcastHub.State.Game
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

        public Dictionary<double, int> goldGraph => BroadcastHubController.Instance.gameController.gameState.GetGoldGraph();
        public List<Inhibitor> inhibitors => BroadcastHubController.Instance.gameController.gameState.GetInhibitors();

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
            return GameController.CurrentSettings.GoldGraph;
        }

        public bool ShouldSerializeinhibitors()
        {
            return GameController.CurrentSettings.Inhibs;
        }

        public bool ShouldSerializebaron()
        {
            return GameController.CurrentSettings.SendBaron;
        }   

        public bool ShouldSerializedragon()
        {
            return GameController.CurrentSettings.SendElder;
        }
    }
}
