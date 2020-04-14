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
        public bool HasClose { get; set; } = true;
        public string OnSave { get; set; }
        public string OnClose { get; set; }
        public string SaveButtonName { get; set; } = "保存";
        public string CloseButtonName { get; set; } = "关闭";
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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var buttons = new StringBuilder("[");
            var id = output.Attributes["id"]?.Value;

            if (HasSave)
                buttons.Append($"{{ text:'{SaveButtonName}', iconCls:'icon-ok', handler:function(){{ {OnSave} }} }},");
            if (HasClose)
                buttons.Append($"{{ text:'{CloseButtonName}', iconCls:'icon-cancel', handler:function(){{ {(id != null ? $"$('#{id}').window('close')" : "")} }} }},");


            buttons.Append("]");

            Options.AddIf(OnClose.IsNotNullOrWhiteSpace(), LayuiConsts.Dialog_OnClose, $"function(){{ {OnClose} }}");

            Options.AddIf(IsMax, LayuiConsts.Dialog_IsMax, IsMax);

            Options.Add(LayuiConsts.Dialog_Buttons, buttons.ToString());

            output.Attributes.Add(LayuiConsts.Dialog_Closed, Closed.ToString().ToCamelCase());

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
