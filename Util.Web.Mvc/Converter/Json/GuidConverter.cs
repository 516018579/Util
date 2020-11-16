using System;
using Newtonsoft.Json;

namespace Util.Web.Mvc.Converter.Json
{
    public class GuidConverter : JsonConverter
    {
        public override bool CanRead => false;
        public static bool IsLower = true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stringValue = ((Guid?)value)?.ToString();
            writer.WriteValue(IsLower ? stringValue?.ToLower() : stringValue?.ToUpper());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid) || objectType == typeof(Guid?);
        }
    }
}
