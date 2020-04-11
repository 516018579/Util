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
    [HtmlTargetElement("filebox")]
    public class FileTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-filebox";
        protected override string TagName => "input";

        public string ButtonText { get; set; }

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(ButtonText.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Button_Text, ButtonText);
            base.AddOption(context, output);
        }
    }
}
