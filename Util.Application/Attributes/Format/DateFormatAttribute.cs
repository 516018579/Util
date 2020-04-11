using System;

namespace Util.Application.Attributes.Format
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DateFormatAttribute : Attribute
    {
        public DateFormatAttribute()
        {
        }

        public DateFormatAttribute(string dateFormatString)
        {
            DateFormatString = dateFormatString;
        }

        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateFormatString { get; set; } = DefaultFormatString;

        public static string DefaultFormatString = "yyyy-MM-dd HH:mm:ss";
    }

}
