using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Util.Json
{
    public static partial class JsonExtensions
    {
        public static string ToCamelCase(this string value)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                {
                    value,
                    null
                }
            }, settings)).First().Key;
        }

        /// <summary>
        /// 对象转json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">对象</param>
        /// <param name="hasQuoteName">是否包含引号</param>
        /// <param name="isCamelCase">是否采用CamelCase命名</param>
        /// <returns></returns>
        public static string ToJsonString<T>(this T value, bool hasQuoteName = false, bool isCamelCase = false) where T : class
        {
            string str = "null";
            if (value != null)
            {
                var jsonSerializer = new JsonSerializer();

                if (isCamelCase)
                {
                    jsonSerializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }

                var stringWriter = new StringWriter();
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.QuoteName = hasQuoteName;
                    jsonTextWriter.QuoteChar = '\'';

                    jsonSerializer.Serialize(jsonTextWriter, value);
                    str = stringWriter.ToString();
                }
            }
            return str;
        }

        public static T To<T>(this string json, bool catchError = false) where T : class, new()
        {
            try
            {
                return string.IsNullOrWhiteSpace(json) ? default(T) : JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                if (catchError)
                {
                    return default(T);
                }
                throw;
            }
        }

        public static T ToEnumerable<T>(this string json, bool catchError = false) where T : IEnumerable, new()
        {
            T objList = new T();
            try
            {
                if (!string.IsNullOrWhiteSpace(json))
                    objList = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                if (!catchError)
                    throw;
            }

            return objList;
        }
    }
}
