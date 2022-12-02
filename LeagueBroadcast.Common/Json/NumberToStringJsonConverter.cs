using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Common.Json
{
    public sealed class NumberToStringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return $"{reader.GetDecimal()}";
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            decimal? decimalValue;
            if (value is null || value.Length == 0)
            {
                decimalValue = 0;
            } else
            {
                decimalValue = decimal.Parse(value);
            }
            writer.WriteNumberValue(decimalValue.Value);
        }
    }
}
