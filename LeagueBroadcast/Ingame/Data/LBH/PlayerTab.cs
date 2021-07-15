using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.Common.Data.RIOT;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.LBH
{
    public class PlayerTab
    {
        public string PlayerName;
        public string IconPath;
        public ValueBar Values;
        public string[] ExtraInfo;

        public static List<PlayerTab> GetEXPTabs()
        {
            var ret = new List<PlayerTab>();
            if (BroadcastController.Instance.IGController.gameData.gameTime < 5)
                return ret;

            BroadcastController.Instance.IGController.gameState.GetAllPlayers().ForEach(p => {
                ret.Add(new PlayerTab()
                {
                    PlayerName = p.summonerName,
                    IconPath = $"Cache\\{DataDragon.version.Champion}\\champion\\{p.championID}_square.png",
                    Values = new() { MinValue = ChampionLevel.Levels[p.level - 1].exp, MaxValue = ChampionLevel.Levels[p.level].exp, CurrentValue = p.farsightObject.EXP },
                    ExtraInfo = new string[] { p.level + "", "exp", p.team }
                });
            });

            return ret;

        }

        public static List<PlayerTab> GetGoldTabs()
        {
            var ret = new List<PlayerTab>();
            if (BroadcastController.Instance.IGController.gameData.gameTime < 5)
                return ret;

            double leastGold = 0;
            double mostGold = 0;
            BroadcastController.Instance.IGController.gameState.GetAllPlayers().ForEach(p =>
            {
                var gold = p.farsightObject.GoldTotal;
                if (gold > mostGold)
                    mostGold = gold;
                if (gold < leastGold)
                    leastGold = gold;
            });
            BroadcastController.Instance.IGController.gameState.GetAllPlayers().ForEach(p =>
            {
                var gold = p.farsightObject.GoldTotal;
                ret.Add(new PlayerTab()
                {
                    PlayerName = p.summonerName,
                    IconPath = $"Cache\\{DataDragon.version.Champion}\\champion\\{p.championID}_square.png",
                    Values = new() { MinValue = Math.Max(0, leastGold - 100), MaxValue = mostGold, CurrentValue = gold },
                    ExtraInfo = new string[] { gold + "", "gold", p.team}
                });
            });

            return ret;
        }

        public static List<PlayerTab> GetCSPerMinTabs()
        {
            var ret = new List<PlayerTab>();
            if (BroadcastController.Instance.IGController.gameData.gameTime < 5)
                return ret;

            double leastCSperMin = 0;
            double mostCSperMin = 0;
            BroadcastController.Instance.IGController.gameState.GetAllPlayers().ForEach(p =>
            {
                var cspm = p.GetCSPerMinute();
                if (cspm > mostCSperMin)
                    mostCSperMin = cspm;
                if (cspm < mostCSperMin)
                    leastCSperMin = cspm;
            });
            BroadcastController.Instance.IGController.gameState.GetAllPlayers().ForEach(p =>
            {
                var cspm = p.GetCSPerMinute();
                ret.Add(new PlayerTab()
                {
                    PlayerName = p.summonerName,
                    IconPath = $"Cache\\{DataDragon.version.Champion}\\champion\\{p.championID}_square.png",
                    Values = new() { MinValue = leastCSperMin, MaxValue = mostCSperMin, CurrentValue = cspm },
                    ExtraInfo = new string[] { cspm + "", "cspm", p.team }
                });
            });

            return ret;
        }
    }

    public class ValueBar
    {
        public double MinValue;
        public double MaxValue;
        public double CurrentValue;
    }
}
