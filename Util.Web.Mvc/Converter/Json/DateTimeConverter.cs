using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Util.Web.Mvc.Converter.Json
{
    public class DateTimeConverter : IsoDateTimeConverter
    {
        public DateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }

        public DateTimeConverter(string dateFormat)
        {
            DateTimeFormat = dateFormat;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }
            if (reader.Value is DateTime time)
            {
                if (time.Kind == DateTimeKind.Utc)
                {
                    time = new DateTime(time.Date.Year, time.Date.Month, time.Date.Day).AddDays(1);
                }
                return time;
            }
            var value = (string)reader.Value;

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.Length == 13 && value.Contains(" "))
            {
                value += "00:00";
            }

            var date = DateTime.Parse(value);

            if (value.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
            {
                date = date.AddHours(8);
            }

            return date;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                if (value is DateTime time && time == default || value is DateTimeOffset offset && offset == default)
                {
                    writer.WriteValue(default(string));
                    return;
                }
            }
            base.WriteJson(writer, value, serializer);
        }
    }
}
