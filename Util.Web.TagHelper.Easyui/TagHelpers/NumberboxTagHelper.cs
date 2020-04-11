using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("easyui-numberbox")]
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
            Options.AddIf(Max.HasValue, EasyuiConsts.ValidType_Max, Max);
            Options.AddIf(Min.HasValue, EasyuiConsts.ValidType_Min, Min);
            Options.AddIf(Precision.HasValue, EasyuiConsts.ValidType_Precision, Precision);
            base.AddOption(context, output);
        }
    }
}
