using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Util.Json
{
    public static partial class JsonExtensions
    {
        public static Dictionary<string, object> ToDictionary(this JObject json)
        {
            return json.ToCollections() as Dictionary<string, object>;
        }

        private static object ToCollections(this object o)
        {
            if (o is JObject jo) return jo.ToObject<IDictionary<string, object>>().ToDictionary(k => k.Key, v => ToCollections(v.Value));
            if (o is JArray ja) return ja.ToObject<List<object>>().Select(ToCollections).ToList();
            return o;
        }
    }
}
