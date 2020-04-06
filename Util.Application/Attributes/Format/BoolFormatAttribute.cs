using System;
using System.Linq;
using Util.Domain;

namespace Util.Application.Attributes.Format
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BoolFormatAttribute : Attribute, IBoolFormat
    {
        public string GetText(bool value)
        {
            return EnumUtil.BoolDictionary[value];
        }

        public bool GetValue(string value)
        {
            return EnumUtil.BoolDictionary.FirstOrDefault(x=>x.Value==value).Key;
        }
    }
}
