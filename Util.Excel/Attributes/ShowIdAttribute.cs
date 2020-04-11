using System;

namespace Util.Excel.Attributes
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
