using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.CommunityDragon
{
    public class Item
    {
        public static HashSet<Item> All { get; set; } = new();

        public static HashSet<Item> Full { get; set; } = new();

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
}
