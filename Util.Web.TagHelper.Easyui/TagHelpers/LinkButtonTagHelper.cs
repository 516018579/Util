using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("easyui-linkbutton")]
    public class LinkButtonTagHelper : EasyuiTagHelper
    {
        protected override string ClassName => "easyui-linkbutton";
        protected override string TagName => "a";

        [HtmlAttributeName(EasyuiConsts.Disabled)]
        public bool? IsDisable { get; set; }
        [HtmlAttributeName(EasyuiConsts.Plain)]
        public bool? IsPlain { get; set; }

        [HtmlAttributeName("Icon")]
        public IconType Icon { get; set; } = IconType.None;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(IsDisable.HasValue, EasyuiConsts.Disabled, IsDisable);
            Options.AddIf(IsPlain.HasValue, EasyuiConsts.Plain, IsPlain);
            output.Attributes.Add("href", "javascript:void(0)");

            if (Icon != IconType.None)
            {
                output.Attributes.Add(EasyuiConsts.Iconcls, $"icon-{Icon.ToString().ToLower()}");
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
