using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.Extensions;
using Util.Json;
using Util.Web;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("textbox")]
    public class TextboxTagHelper : EasyuiTagHelper
    {
        protected override string ClassName => "easyui-textbox";
        protected override string TagName => "input";
        [HtmlAttributeName(WebConsts.Easyui.Editable)]
        public virtual bool? IsEdit { get; set; }

        [HtmlAttributeName(WebConsts.Easyui.Multiline)]
        public virtual bool? IsMultiline { get; set; }

        [HtmlAttributeName(WebConsts.Easyui.Disabled)]
        public virtual bool? IsDisable { get; set; }

        [HtmlAttributeName(WebConsts.Easyui.Required)]
        public virtual bool? IsRequired { get; set; }
        public virtual string ValidType { get; set; }
        public virtual bool? Clear { get; set; }
        public virtual int MaxLength { get; set; } = 50;

        protected List<string> ValidTypes = new List<string>();

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(IsRequired.HasValue, WebConsts.Easyui.Required, IsRequired);
            Options.AddIf(IsMultiline.HasValue, WebConsts.Easyui.Multiline, IsMultiline);
            Options.AddIf(IsDisable.HasValue, WebConsts.Easyui.Disabled, IsDisable);
            Options.AddIf(IsEdit.HasValue, WebConsts.Easyui.Editable, IsEdit);

            if (MaxLength > 0)
            {
                ValidTypes.Add($"{WebConsts.Easyui.ValidType_MaxLength}[{MaxLength}]");
            }

            if (Options.ContainsKey(WebConsts.Easyui.ValidType))
            {
                var valids = Options[WebConsts.Easyui.ValidType].ToString().Split(',').Where(x => x.IsNotNullOrWhiteSpace());
                ValidTypes.AddRange(valids);
            }

            if (ValidType.IsNotNullOrWhiteSpace())
            {
                var valids = ValidType.Split(',').Where(x => x.IsNotNullOrWhiteSpace());
                ValidTypes.AddRange(valids);
            }


            if (IsDisable == true)
                Clear = false;
            if (Clear.HasValue)
                output.Attributes.Add(WebConsts.Easyui.Clear, Clear.ToString().ToCamelCase());

            Options.AddOrUpdate(WebConsts.Easyui.ValidType, $"'{ValidTypes.JoinAsString()}'");
        }
    }
}
