using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Ingame.Data.Frontend;
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
        #endregion 

        public FrontEndObjective dragon;

        public FrontEndObjective baron;

        public double gameTime;
        public bool gamePaused;

        public float blueGold => gameState.blueTeam.GetGold(gameTime);
        public float redGold => gameState.redTeam.GetGold(gameTime);

        public Dictionary<double, float> goldGraph => gameState.GetGoldGraph();
        public InhibitorInfo inhibitors;

        public ScoreboardConfig scoreboard;

        public InfoSidePage infoPage;
        public string blueColor => gameState.blueTeam.color;
        public string redColor => gameState.redTeam.color;

        public StateData()
        {
            this.gameState = BroadcastController.Instance.IGController.gameState;
            this.dragon = new (Objective.ObjectiveType.Dragon, 300);
            this.baron = new (Objective.ObjectiveType.Baron, 1200);
            this.backBaron = new(0);
            this.backDragon = new(0);
            this.scoreboard = new ScoreboardConfig();
            this.inhibitors = new InhibitorInfo();
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

        public bool ShouldSerializeinfoPage()
        {
            return IngameController.CurrentSettings.SideGraph;
        }
    }
}
