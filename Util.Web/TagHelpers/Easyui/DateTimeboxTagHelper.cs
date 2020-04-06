using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("datetimebox")]
    public class DateTimeboxTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-datetimebox";
        protected override string TagName => "input";
    }
}
