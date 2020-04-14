using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;
using Util.Web.TagHelpers.Extensions;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-switch")]
    public class SwitchTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "layui-switch";
        protected override string TagName => "input";
        public string Label { get; set; }
        public bool IsChecked { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.AddOrUpdate("type", "checkbox");
            output.Attributes.Add(LayuiConsts.LaySkin, "switch");

            if (Label.IsNotNullOrWhiteSpace())
                output.Attributes.Add(LayuiConsts.LayText, Label);
            if (IsChecked)
                output.Attributes.Add("checked", null);
            if (IsDisable == true)
                output.Attributes.Add("disabled", null);

            return base.ProcessAsync(context, output);
        }
    }
}
