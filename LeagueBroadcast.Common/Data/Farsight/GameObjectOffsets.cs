using LeagueBroadcast.Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Data.Farsight
{
    public class GameObjectOffsets
    {
        [JsonConverter(typeof(HexStringJsonConverter))]
        public int ID { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int NetworkID { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Team { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Pos { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Mana { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int ManaMax { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Health { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int HealthMax { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int GoldCurrent { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int GoldTotal { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Experience { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int ChampionName { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int ItemList { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int ItemListItem { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int ItemInfo { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int ItemInfoId { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int PlayerNameLocation { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int PlayerNameLength { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int PlayerNameMaxLength { get; set; }
    }
}
