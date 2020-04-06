using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using Util.Extensions;
using Util.Web;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("linkbutton")]
    public class LinkButtonTagHelper : EasyuiTagHelper
    {
        protected override string ClassName => "easyui-linkbutton";
        protected override string TagName => "a";

        [HtmlAttributeName(WebConsts.Easyui.Disabled)]
        public bool? IsDisable { get; set; }
        [HtmlAttributeName(WebConsts.Easyui.Plain)]
        public bool? IsPlain { get; set; }

        [HtmlAttributeName("Icon")]
        public IconType Icon { get; set; } = IconType.None;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(IsDisable.HasValue, WebConsts.Easyui.Disabled, IsDisable);
            Options.AddIf(IsPlain.HasValue, WebConsts.Easyui.Plain, IsPlain);
            output.Attributes.Add("href", "javascript:void(0)");

            if (Icon != IconType.None)
            {
                output.Attributes.Add(WebConsts.Easyui.Iconcls, $"icon-{Icon.ToString().ToLower()}");
            }

            await base.ProcessAsync(context, output);
        }

    }
    public enum IconType
    {
        Add,
        Edit,
        Remove,
        Save,
        Reload,
        Cut,
        Ok,
        No,
        Cancel,
        Search,
        Print,
        Help,
        Undo,
        Redo,
        Back,
        TIp,
        Down,
        Check,
        Publish,
        Import,
        None,
        Clear
    }
}
