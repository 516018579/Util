using System;

namespace Util.Web.Attributes.Control
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrecisionAttribute : Attribute
    {
        public PrecisionAttribute()
        {
        }

        public PrecisionAttribute(int precision)
        {
            Precision = precision;
        }

        public int Precision { get; set; } = 2;
    }
}
