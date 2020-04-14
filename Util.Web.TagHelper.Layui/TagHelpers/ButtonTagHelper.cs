using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("easyui-button")]
    public class ButtonTagHelper : LayuiTagHelper
    {
        protected override string ClassName => "layui-btn";
        protected override string TagName => "button";

        /// <summary>
        /// 按钮主题
        /// </summary>
        public ButtonTheme? Theme { get; set; }

        /// <summary>
        /// 是否是圆角
        /// </summary>
        public bool IsRadius { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
           
            Options.AddIf(Theme.HasValue, "radius", true);
            if (Theme.HasValue)
            {
                Class.Add($"layui-btn-{Theme.ToString().ToLower()}");
            }

            await base.ProcessAsync(context, output);
        }

        protected override void InitOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(IsRadius, "radius", true);
            base.InitOption(context, output);
        }
    }

    public enum ButtonTheme
    {
        Primary,
        Normal,
        Warm,
        Danger,
        Disabled
    }

    public enum ButtonSzie
    {
        Lg,
        Sm,
        Xs
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
