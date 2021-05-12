using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.LBH;
using LeagueBroadcast.Ingame.Data.LBH.Objectives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.State
{
    public class StateData
    {

        #region NotSerialized
        [JsonIgnore]
        public BackEndObjective backBaron;

        [JsonIgnore]
        public BackEndObjective backDragon;
        private State gameState;

        public FrontEndObjective dragon;

        public FrontEndObjective baron;
        #endregion 


        public List<string> blueDragons => gameState.blueTeam.dragonsTaken;
        public List<string> redDragons => gameState.redTeam.dragonsTaken;

        public double gameTime;
        public bool gamePaused;

        public float blueGold => gameState.blueTeam.GetGold();
        public float redGold => gameState.redTeam.GetGold();

        public Dictionary<double, float> goldGraph => gameState.GetGoldGraph();
        public List<Inhibitor> inhibitors => gameState.GetInhibitors();

        public StateData()
        {
            this.gameState = BroadcastController.Instance.IGController.gameState;
            this.dragon = new (Objective.ObjectiveType.Dragon);
            this.baron = new (Objective.ObjectiveType.Baron);
            this.backBaron = new(0);
            this.backDragon = new(0);
        }

        public bool ShouldSerializegoldGraph()
        {
            return IngameController.CurrentSettings.GoldGraph;
        }

        public bool ShouldSerializeinhibitors()
        {
            return IngameController.CurrentSettings.Inhibs;
        }

        public bool ShouldSerializebaron()
        {
            return IngameController.CurrentSettings.Baron && backBaron.DurationRemaining > 0;
        }

        public bool ShouldSerializedragon()
        {
            return IngameController.CurrentSettings.Elder && backDragon.DurationRemaining > 0;
        }
    }
}
