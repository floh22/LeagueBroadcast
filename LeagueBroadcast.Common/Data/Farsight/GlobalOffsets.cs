using LeagueBroadcast.Common.Json;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Data.Farsight
{
    public class GlobalOffsets
    {
        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Manager { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int HUDInstance { get; set; }

        public ObjectListOffsets ObjectLists { get; set; } = new();

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int MapCount { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int MapRoot { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int MapNodeNetId { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int MapNodeObject { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int GameTime { get; set; }

        
    }

    public class ObjectListOffsets
    {
        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Hero { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Minion { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Turret { get; set; }

        [JsonConverter(typeof(HexStringJsonConverter))]
        public int Inhibitor { get; set; }
    }
}
