using System;
using System.ComponentModel;
using System.Reflection;

namespace Util.Web.Attributes.Control
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComboboxAttribute : Attribute
    {
        public ComboboxAttribute()
        {
            Type = null;
            IsLoadData = false;
        }

        public ComboboxAttribute(Type type, bool isLoadData = true)
        {
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
            Type = type;
            DisplayName = attribute == null ? type.Name : attribute.DisplayName;
            IsLoadData = isLoadData;
        }

        public Type Type { get; set; }
        public bool IsLoadData { get; set; }
        public string DisplayName { get; set; }
    }
}
