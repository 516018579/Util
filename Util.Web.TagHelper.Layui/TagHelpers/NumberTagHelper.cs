using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AngleSharp.Common;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;
using Util.Web.TagHelpers.Extensions;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-textarea")]
    public class NumberTagHelper : TextboxTagHelper
    {
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.AddOrUpdate("type", "number");
            return base.ProcessAsync(context, output);
        }

        protected override void InitValids(TagHelperOutput output)
        {
            Valids.Add("number");
            base.InitValids(output);
        }
    }
}
