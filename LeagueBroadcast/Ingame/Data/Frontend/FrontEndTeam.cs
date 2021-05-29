using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.Frontend
{
    public class FrontEndTeam
    {
        private bool mapSide;

        #region TeamProperties
        public string Name;
        public string Icon;
        public int Score;
        #endregion

        #region Scoreboard
        public int Kills;
        public int Towers;
        public float Gold;
        public List<string> Dragons { get { return mapSide ? BroadcastController.Instance.IGController.gameState.redTeam.dragonsTaken : BroadcastController.Instance.IGController.gameState.blueTeam.dragonsTaken; } }
        #endregion

        public FrontEndTeam(string tag, bool mapSide)
        {
            this.mapSide = mapSide;

            this.Score = 0;
            this.Icon = TeamConfigViewModel.DefaultIconPath;
            this.Name = tag;

            this.Kills = 0;
            this.Towers = 0;
            this.Gold = 2500;
        }

        #region SerializeConditions
        public bool ShouldSerializeIcon()
        {
            return IngameController.CurrentSettings.TeamIcons;
        }

        public bool ShouldSerializeScore()
        {
            return IngameController.CurrentSettings.TeamStats;
        }

        public bool ShouldSerializeName()
        {
            return IngameController.CurrentSettings.TeamNames;
        }

        public bool ShouldSerializeKills()
        {
            return ConfigController.Component.Ingame.UseCustomScoreboard;
        }

        public bool ShouldSerializeTowers()
        {
            return ConfigController.Component.Ingame.UseCustomScoreboard;
        }

        public bool ShouldSerializeGold()
        {
            return ConfigController.Component.Ingame.UseCustomScoreboard;
        }
        #endregion
    }
}
