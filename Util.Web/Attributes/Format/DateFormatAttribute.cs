using System;

namespace Util.Web.Attributes.Format
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

        public string DateFormatString { get; set; } = DefaultFormatString;

        public static string DefaultFormatString = "yyyy-MM-dd HH:mm:ss";
    }

}
