using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Application.Attributes.Easyui
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColOptionAttribute : Attribute
    {
        public string Formatter { get; set; }
        public Func<object, string> FormatterFunc { get; set; }
        public int Width { get; set; }
        public bool Sortable { get; set; }
    }
}
