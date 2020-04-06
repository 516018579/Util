using System;

namespace Util.Application.Attributes.Easyui
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DateTimeBoxAttribute : Attribute
    {
        public DateTimeBoxAttribute()
        {
        }

        public DateTimeBoxAttribute(string dateFormatString)
        {
            DateFormatString = dateFormatString;
        }

        public string DateFormatString { get; set; } = "yyyy-MM-dd HH:mm:ss";
    }
}
