using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebhookGail.Middleware
{
    public class CustomDateTimeConverter : JsonConverter<DateTime?>
    {
        private const string CustomDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            string dateString = reader.GetString();
            if (DateTime.TryParseExact(dateString, CustomDateFormat, null, System.Globalization.DateTimeStyles.RoundtripKind, out var date))
            {
                return date;
            }

            throw new JsonException($"Invalid date format: {dateString}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString(CustomDateFormat));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
