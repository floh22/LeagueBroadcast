using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.RIOT
{
    public class ItemData
    {
        public int itemID;
        public ItemCost gold;
        public int specialRecipe;
        public string sprite;

        public ItemData(int itemID)
        {
            this.itemID = itemID;
        }
    }
}
