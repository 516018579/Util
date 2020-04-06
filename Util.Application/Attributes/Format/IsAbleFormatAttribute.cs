using System;
using System.Linq;
using Util.Domain;

namespace Util.Application.Attributes.Format
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsAbleFormatAttribute : Attribute, IBoolFormat
    {
        public string GetText(bool value)
        {
            return EnumUtil.IsAbleDictionary[value];
        }

        public bool GetValue(string value)
        {
            return EnumUtil.IsAbleDictionary.FirstOrDefault(x => x.Value == value).Key;
        }
    }
}
