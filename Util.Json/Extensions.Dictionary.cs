using System.Collections.Generic;
using System.Linq;

namespace Util.Json
{
    public static partial class JsonExtensions
    {
        public static string GetJsonTextValueString<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
        {
            return dictionary.Select(x => new { value = x.Key, text = x.Value }).ToJsonString();
        }
    }
}
