using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        ///     添加一个带有所提供的键和值的元素。(如果值存在,则覆盖原来的值)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static void AddIf<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool isAdd, TKey key, TValue value)
        {
            if (isAdd)
            {
                dictionary.Add(key, value);
            }
        }


        /// <summary>
        /// 将字典的值转换为某个类型
        /// 如果Key不存在则返回T类型的默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValueTo<T>(this IDictionary dictionary, string key)
        {
            T value = default(T);
            try
            {
                if (dictionary.Keys.Cast<string>().Any(dictionaryKey => dictionaryKey == key))
                {
                    if (dictionary[key] != null)
                    {
                        value = (T)Convert.ChangeType(dictionary[key], typeof(T));
                    }
                    return value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return value;
        }

        /// <summary>
        /// 字典集合转DataTable
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionaryList"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TValue>(this IEnumerable<Dictionary<string, TValue>> dictionaryList)
        {
            var dt = new DataTable();
            foreach (var dictionary in dictionaryList)
            {
                var dataRow = dt.NewRow();
                foreach (var column in dictionary.Keys)
                {
                    if (!dt.Columns.Contains(column))
                        dt.Columns.Add(column);
                    dataRow[column] = dictionary[column];
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        public static TKey GetFirstKeyOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> source, TValue value)
        {
            TKey key = default;
            foreach (var item in source)
            {
                if (item.Value.Equals(value))
                {
                    key = item.Key;
                    break;
                }
            }

            return key;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.ToDictionary(x => x.Key, x => x.Value);
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue obj;
            return !dictionary.TryGetValue(key, out obj) ? default(TValue) : obj;
        }
    }
}
