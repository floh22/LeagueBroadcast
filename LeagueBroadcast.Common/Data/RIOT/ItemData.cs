using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Data.RIOT
{
    public class ItemData
    {
        public static List<ItemData> Items = new();

        public int itemID;
        public ItemCost gold;
        public int specialRecipe;
        public string sprite;
        public string name;

        public ItemData(int itemID)
        {
            this.itemID = itemID;
        }
    }
}
