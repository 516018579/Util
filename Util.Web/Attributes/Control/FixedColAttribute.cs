using System;

namespace Util.Web.Attributes.Control
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FixedColAttribute : Attribute
    {
        public FixedColAttribute(string @fixed = null)
        {
            Fixed = @fixed;
        }

        public string Fixed { get; set; }
    }
}
