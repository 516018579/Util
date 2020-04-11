using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// 字符串转日期类型
        /// </summary>
        /// <param name="date"></param>
        /// <param name="formatStr"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string date, string formatStr = null)
        {
            return formatStr == null ? DateTime.Parse(date) : DateTime.ParseExact(date, formatStr, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 字符串转日期类型
        /// </summary>
        /// <param name="date"></param>
        /// <param name="errorDate">转换失败时返回的值</param>
        /// <param name="formatStr"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string date, DateTime errorDate, string formatStr = null)
        {
            try
            {
                errorDate = date.ToDateTime(formatStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return errorDate;
        }

        /// <summary>
        /// 添加字符串到集合,并返回值
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="list">集合</param>
        /// <returns>当前值</returns>
        public static string AddToList<T>(this string value, T list) where T : ICollection<string>
        {
            list.Add(value);
            return value;
        }

        /// <summary>
        /// 去掉开头字符串,如果字符存在
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charper">要去掉的字符</param>
        /// <returns></returns>
        public static string TrimFirst(this string value, string charper)
        {
            try
            {
                if (value.IndexOf(charper, StringComparison.Ordinal) == 0)
                {
                    return value.Substring(charper.Length);
                }
            }
            catch (Exception e)
            { }
            return value;
        }

        /// <summary>
        /// 去掉结尾字符串,如果字符存在
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charper">要去掉的字符</param>
        /// <returns></returns>
        public static string TrimLast(this string value, string charper)
        {
            try
            {
                if (value.LastIndexOf(charper, StringComparison.Ordinal) == value.Length - charper.Length)
                {
                    return value.Substring(0, value.Length - charper.Length);
                }
            }
            catch (Exception e)
            { }
            return value;
        }

        public static string TrimAll(this string value, params string[] charpers)
        {
            return charpers.Aggregate(value, (current, charper) => current.Replace(charper, ""));
        }

        /// <summary>
        /// 获取2个字符中间的字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="star">开头字符</param>
        /// <param name="end">结尾字符</param>
        /// <returns></returns> 
        public static string GetCenterValue(this string str, string star, string end)
        {
            var rg = new Regex("(?<=(" + Regex.Escape(star) + "))[.\\s\\S]*?(?=(" + Regex.Escape(end) + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }

        public static string GetLastString(this string value, int length)
        {
            if (length > value.Length)
                length = value.Length;
            return value.ToString(null).IsNotNullOrWhiteSpace() ? value : value.Substring(value.Length - length, length);
        }

        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNumber(this string number)
        {
            return double.TryParse(number, out _);
        }
    }
}
