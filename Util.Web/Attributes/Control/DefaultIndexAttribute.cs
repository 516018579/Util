using System;

namespace Util.Web.Attributes.Control
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultIndexAttribute : Attribute
    {
        public DefaultIndexAttribute()
        {
        }

        public DefaultIndexAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; set; }
    }
}
