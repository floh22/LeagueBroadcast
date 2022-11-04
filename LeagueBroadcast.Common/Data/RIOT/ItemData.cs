using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.RIOT
{
    public class ItemData
    {
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


public class CDragonItem
{

    public static HashSet<CDragonItem> All { get; set; } = new();

    public static HashSet<CDragonItem> Full { get; set; } = new();


    [JsonPropertyName("id")]
    public int ID { get; set; } = -1;

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("from")]
    public List<int> Components { get; set; } = new();

    [JsonPropertyName("price")]
    public int Price { get; set; } = 0;

    [JsonPropertyName("priceTotal")]
    public int PriceTotal { get; set; } = 0;

    [JsonPropertyName("specialRecipe")]
    public int SpecialRecipe { get; set; } = 0;

    [JsonPropertyName("iconPath")]
    public string IconPath { get; set; } = "";
}
