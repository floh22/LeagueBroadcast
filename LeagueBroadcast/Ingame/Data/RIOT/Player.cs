using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Farsight.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.RIOT
{
    public class Player
    {
        public int id;
        public string summonerName;
        public string championName;
        public bool isDead;
        public IEnumerable<Item> items;
        public int level;
        public string position;
        public float respawnTimer;
        public RuneList runes;
        public Score scores;
        public SummonerList summonerSpells;
        public string team;

#nullable enable
        public GameObject? farsightObject;
        public Dictionary<double, int> csHistory = new();
        public Dictionary<double, float> goldHistory = new();
#nullable disable

        
        public bool diedDuringBaron = false;
        public bool diedDuringElder = false;

        public Player()
        {
            csHistory[0] = 0;
            goldHistory[0] = 500;
        }

        public float GetCSPerMinute(float gameTime)
        {
            return (float)(scores.creepScore / gameTime);
        }

        public void UpdateInfo(Player p)
        {
            this.isDead = p.isDead;
            this.items = p.items;
            this.level = p.level;
            this.respawnTimer = p.respawnTimer;
            this.scores.Update(p.scores, !ConfigController.Component.Ingame.UseLiveEvents);
        }

        [ObsoleteAttribute("Does not work in custom games", true)]
        public void UpdateId(string position, int teamId)
        {
            int posId = 0;
            switch (position)
            {
                case "TOP":
                    posId = 0;
                    break;
                case "JUNGLE":
                    posId = 1;
                    break;
                case "MIDDLE":
                    posId = 2;
                    break;
                case "BOTTOM":
                    posId = 3;
                    break;
                case "UTILITY":
                    posId = 4;
                    break;
            }
            this.id = (teamId == 0 ? 0 : 5) + posId;
        }
    }
}
