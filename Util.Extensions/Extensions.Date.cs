using System;
using System.Globalization;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// DateTime转string类型(格式"yyyy-MM-dd HH:mm:ss")
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// DateTime转string类型(格式"yyyy-MM-dd")
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime? dateTime)
        {
            return dateTime?.ToDateTimeString() ?? "";
        }

        /// <summary>
        /// DateTime转string类型(格式"yyyy-MM-dd")
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        /// <summary>
        /// DateTime转string类型(格式"yyyy-MM-dd")
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime? dateTime)
        {
            return dateTime?.ToDateString() ?? "";
        }

        ///   <summary> 
        ///  获取某一日期是该年中的第几周
        ///   </summary> 
        ///   <param name="dt"> 日期 </param> 
        ///   <returns> 该日期在该年中的周数 </returns> 
        public static int GetWeekOfYear(this DateTime dateTime)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return gc.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        /// <summary>
        /// 获取1970年到指定时间的毫秒数
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public static long GetMillisecond(this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static DateTime SetHour(this DateTime dateTime, int hour)
        {
            return dateTime.AddHours(-dateTime.Hour + hour);
        }

        /// <summary>
        /// 格式化日期为(yyyyMMddHHmmssfff)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToNumberString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmssfff");
        }
    }
}
