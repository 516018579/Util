using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-datebox")]
    public class DateboxTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "layui-datebox";

        /// <summary>
        /// 时间类型
        /// </summary>
        public DateType? DateType { get; set; }

        /// <summary>
        /// 日期格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 是否是日期范围
        /// </summary>
        public bool IsRange { get; set; } = false;
        public string Max { get; set; }
        public string Min { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string Value { get; set; }

        protected override void InitOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(DateType.HasValue, "type", DateType.ToString().ToLower());
            Options.AddIf(Format.IsNotNullOrWhiteSpace(), "format", Format);
            Options.AddIf(IsRange, "range", "true");

            Options.AddIf(Max.IsNotNullOrWhiteSpace(), "max", Max);
            Options.AddIf(Max.IsNotNullOrWhiteSpace(), "min", Min);
            Options.AddIf(Value.IsNotNullOrWhiteSpace(), "value", Value);

            base.InitOption(context, output);
        }

        protected override void InitValids(TagHelperOutput output)
        {
            Valids.Add("date");
            base.InitValids(output);
        }
    }

    public enum DateType
    {
        Year,
        Month,
        Time,
        DateTime
    }
}
