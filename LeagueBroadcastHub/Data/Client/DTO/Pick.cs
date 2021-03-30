using LeagueBroadcastHub.Data.Game.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Data.Client.DTO
{
    public class Pick : PickBan
    {
        public int id;
        public SummonerSpell spell1;
        public SummonerSpell spell2;
        public bool isActive = false;
        public string displayName = "";

        public Pick(int id)
        {
            this.id = id;
        }
    }
}
