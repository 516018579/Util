using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;

namespace Util.Web.TagHelper.Easyui
{
    [HtmlTargetElement("filebox")]
    public class FileTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-filebox";
        protected override string TagName => "input";

        public string ButtonText { get; set; }

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(ButtonText.IsNotNullOrWhiteSpace(), EasyuiConsts.Button_Text, ButtonText);
            base.AddOption(context, output);
        }
    }
}
