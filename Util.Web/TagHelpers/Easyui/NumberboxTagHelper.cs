using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Extensions;
using Util.Web;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("numberbox")]
    public class NumberboxTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-numberbox";
        protected override string TagName => "input";
        public int? Min { get; set; } = 0;
        public int? Max { get; set; }
        /// <summary>
        /// 小时位数
        /// </summary>
        public int? Precision { get; set; }

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(Max.HasValue, WebConsts.Easyui.ValidType_Max, Max);
            Options.AddIf(Min.HasValue, WebConsts.Easyui.ValidType_Min, Min);
            Options.AddIf(Precision.HasValue, WebConsts.Easyui.ValidType_Precision, Precision);
            base.AddOption(context, output);
        }
    }
}
