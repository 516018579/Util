﻿using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Util.Web.TagHelper.Easyui
{
    [HtmlTargetElement("datebox")]
    public class DateboxTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-datebox";
        protected override string TagName => "input";
    }
}
