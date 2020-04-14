using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-dialog")]
    public class DialogTagHelper : LayuiTagHelper
    {
        protected override string ClassName => "layui-dialog";
        protected override string TagName => "div";
        public bool Closed { get; set; } = true;
        public bool HasSave { get; set; } = true;
        public string OnSave { get; set; }
        public string OnClose { get; set; }
        public string OnShow { get; set; }
        public string SaveButtonName { get; set; } = "保存";
        public bool IsMax { get; set; }
        public string Title { get; set; }
        public DialogType Type { get; set; }
        /// <summary>
        /// class名称
        /// </summary>
        public string Skin { get; set; }
        /// <summary>
        /// 宽高
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// Iframe地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 弹出坐标
        /// </summary>
        public string Offset { get; set; }

        /// <summary>
        /// 是否显示最大最小按钮
        /// </summary>
        public bool ShowMaxMin { get; set; } = true;

        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Closed)
            {
                var style = output.Attributes["style"]?.ToString() ?? "";
                if (style.IsNotNullOrWhiteSpace() && !style.EndsWith(";"))
                {
                    style += ";";
                }
                style += "display: none;";
                output.Attributes.SetAttribute("style", style);
            }

            await base.ProcessAsync(context, output);
        }

        protected override void InitOption(TagHelperContext context, TagHelperOutput output)
        {
            var content = output.Content.GetContent(HtmlEncoder.Default);
            if (Url.IsNotNullOrWhiteSpace())
            {
                Type = DialogType.Frame;
                content = Url;
            }
            else if (!content.IsNotNullOrWhiteSpace())
            {
                Type = DialogType.Page;
            }

            Options.Add("type", Type);
            Options.AddIf(Title.IsNotNullOrWhiteSpace(), "title", Title);
            Options.AddIf(content.IsNotNullOrWhiteSpace(), LayuiConsts.Dialog_Content, content);
            Options.AddIf(Area.IsNotNullOrWhiteSpace(), "area", Area);
            if (HasSave)
            {
                Options.Add("btn", new[] { SaveButtonName });
                if (OnSave.IsNotNullOrWhiteSpace())
                {
                    Options.Add("yes", GetJavaScriptString($"function(){{ {OnSave} }} }}"));
                }
            }

            Options.AddIf(OnClose.IsNotNullOrWhiteSpace(), LayuiConsts.Dialog_OnClose, GetJavaScriptString($"function(){{ {OnClose} }}"));
            Options.AddIf(OnShow.IsNotNullOrWhiteSpace(), LayuiConsts.Dialog_OnShow, GetJavaScriptString($"function(){{ {OnShow} }}"));
            Options.AddIf(IsMax, LayuiConsts.Dialog_IsMax, IsMax);
            Options.AddIf(ShowMaxMin, LayuiConsts.Dialog_IsMaxMin, true);
            Options.AddIf(MaxWidth > 0, "maxWidth", MaxWidth);
            Options.AddIf(MaxHeight > 0, "maxHeight", MaxHeight);

            base.InitOption(context, output);
        }
    }

    public enum DialogType
    {
        Info,
        Page,
        Frame,
        Load,
        Tips
    }
}
