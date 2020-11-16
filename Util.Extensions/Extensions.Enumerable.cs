using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// 把字符串拼接为以符合分割的字符串
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="separator">分割符合</param>
        /// <param name="starStr">在每组开头要添加的字符串</param>
        /// <param name="endStr">在每组结尾要添加的字符串</param>
        /// <returns></returns>d
        public static string JoinAsString(this IEnumerable<string> collection, char separator = ',', string starStr = "", string endStr = "")
        {
            return collection.Aggregate("", (current, item) => current + starStr + item + endStr + separator).TrimEnd(separator);
        }

        /// <summary>
        /// 集合去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            DataTable dtReturn = new DataTable();

            var enumerable = list as IList<T> ?? list.ToList();
            if (list != null && enumerable.Any())
            {
                PropertyInfo[] oProps = null;

                foreach (T rec in enumerable)
                {
                    if (oProps == null)
                    {
                        oProps = rec.GetType().GetProperties();

                        foreach (PropertyInfo pi in oProps)
                        {
                            Type colType = pi.PropertyType;
                            if (colType.IsGenericType && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                            {
                                colType = colType.GetGenericArguments()[0];
                            }

                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                        }
                    }

                    DataRow dr = dtReturn.NewRow();
                    foreach (PropertyInfo pi in oProps)
                    {
                        dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
                    }

                    dtReturn.Rows.Add(dr);
                }
            }



            return dtReturn;
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int nSize = 1000)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        public static void Remove<TSource>(this ICollection<TSource> source, Func<TSource, bool> predicate)
        {
            var removes = source.Where(predicate) as ICollection<TSource>;

            source.Remove(removes);
        }

        public static void Remove<TSource>(this ICollection<TSource> source, ICollection<TSource> removes)
        {
            foreach (var item in removes)
            {
                source.Remove(item);
            }
        }

        /// <summary>
        /// 获取最大值, 没有是返回默认值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult MaxOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var enumerable = source.ToList();
            return enumerable.Any() ? enumerable.Max(selector) : default;
        }

        /// <summary>
        /// 获取最值, 没有是返回默认值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult MinOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var enumerable = source.ToList();
            return enumerable.Any() ? enumerable.Min(selector) : default;
        }
    }
}
