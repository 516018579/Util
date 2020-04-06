using System;

namespace Util.Application.Attributes.Easyui
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
