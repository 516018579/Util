using System;

namespace Util.Application.Attributes.Excel
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShowIdAttribute : Attribute
    {
        public ShowIdAttribute()
        {
        }

        public ShowIdAttribute(string idName)
        {
            IdName = idName;
        }

        public string IdName { get; set; } = "编号";
    }
}
