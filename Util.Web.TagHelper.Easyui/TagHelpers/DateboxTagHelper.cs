using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("easyui-datebox")]
    public class DateboxTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-datebox";
        protected override string TagName => "input";
    }
}
