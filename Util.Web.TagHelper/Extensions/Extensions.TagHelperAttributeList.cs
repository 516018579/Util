using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Util.Web.TagHelpers.Extensions
{
    public static partial class TagHelperExtensions
    {
        public static void AddOrUpdate(this TagHelperAttributeList attributes, string name, object value)
        {
            if (attributes.ContainsName(name))
            {
                attributes.SetAttribute(name, value);
            }
            else
            {
                attributes.Add(name, value);
            }
        }

        public static void AddOrUpdate(this TagHelperAttributeList attributes, string name, Func<object, object> valueFunc)
        {
            object value;
            if (attributes.ContainsName(name))
            {
                value = valueFunc(attributes[name].Value);
                attributes.SetAttribute(name, value);
            }
            else
            {
                value = valueFunc(null);
                attributes.Add(name, value);
            }
        }
    }
}
