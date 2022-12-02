using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LeagueBroadcast.Common.Json
{
    public sealed class HexStringJsonConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string num = reader.GetString() ?? "";
            int hexNum;
            if (num.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) || num.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase))
                num = num.Substring(2);

            bool parsedSuccessfully = int.TryParse(num, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out hexNum);
            if (parsedSuccessfully)
                return hexNum;
            return -1;
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"0x{value:x}");
        }
    }
}
