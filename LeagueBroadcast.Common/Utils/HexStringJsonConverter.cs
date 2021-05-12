using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace LeagueBroadcast.Common.Utils
{
    public sealed class HexStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(int).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue($"0x{value:x}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(reader.ValueType == typeof(Int32) || reader.ValueType == typeof(Int64))
            {
                return Convert.ToInt32(reader.Value);
            }
            var res = Int32.Parse(reader.Value.ToString().Remove(0,2), System.Globalization.NumberStyles.HexNumber);
            return res;
        }
    }
}
