using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Util.Domain;
using Util.Json;

namespace Util.Web.Mvc.Extensions
{
    public static class MvcExtensions
    {
        public static IHtmlContent GetEnumJsonString(this IHtmlHelper htmlHelper, Type enumType, Func<KeyValuePair<int, string>, object> selector = null)
        {
            object DefaultSelector(KeyValuePair<int, string> x) => new {Value = x.Key, Label = x.Value};

            return htmlHelper.Raw(EnumUtil.GetEnumValueList(enumType).Select(selector ?? DefaultSelector).ToJsonString(isCamelCase: true));
        }

        public static IHtmlContent Serialize(this IHtmlHelper htmlHelper, object value)
        {
            return htmlHelper.Raw(JsonConvert.SerializeObject(value));
        }
    }
}
