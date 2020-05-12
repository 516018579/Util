using System;

namespace Util.Application.Attributes.Control
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormItemAttribute : Attribute
    {
        public FormItemAttribute(string @group = null, string width = null, string before = null, string after = null)
        {
            Group = @group;
            Width = width;
            Before = before;
            After = after;
        }

        public FormItemAttribute(uint colSpan, string @group = null, string width = null, string before = null, string after = null)
        {
            Group = @group;
            ColSpan = colSpan;
            Width = width;
            Before = before;
            After = after;
        }

        public FormItemAttribute()
        {
        }

        public string Group { get; set; }
        public uint? ColSpan { get; set; }
        public string Width { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }
}
