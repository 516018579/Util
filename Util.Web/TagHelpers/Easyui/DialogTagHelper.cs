using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using Util.Extensions;
using Util.Web;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("dialog")]
    public class DialogTagHelper : EasyuiTagHelper
    {
        protected override string ClassName => "easyui-dialog";
        protected override string TagName => "div";
        public bool Closed { get; set; } = true;
        public bool HasSave { get; set; } = true;
        public bool HasClose { get; set; } = true;
        public string OnSave { get; set; }
        public string OnClose { get; set; }
        public string SaveButtonName { get; set; } = "保存";
        public string CloseButtonName { get; set; } = "关闭";
        public bool IsMax { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var buttons = new StringBuilder("[");
            var id = output.Attributes["id"]?.Value;

            if (HasSave)
                buttons.Append($"{{ text:'{SaveButtonName}', iconCls:'icon-ok', handler:function(){{ {OnSave} }} }},");
            if (HasClose)
                buttons.Append($"{{ text:'{CloseButtonName}', iconCls:'icon-cancel', handler:function(){{ {(id != null ? $"$('#{id}').window('close')" : "")} }} }},");


            buttons.Append("]");

            Options.AddIf(OnClose.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Dialog_OnClose, $"function(){{ {OnClose} }}");

            Options.AddIf(IsMax, WebConsts.Easyui.Dialog_IsMax, IsMax);

            Options.Add(WebConsts.Easyui.Dialog_Buttons, buttons.ToString());

            output.Attributes.Add(WebConsts.Easyui.Dialog_Closed, Closed.ToString().ToCamelCase());

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

    }
}
