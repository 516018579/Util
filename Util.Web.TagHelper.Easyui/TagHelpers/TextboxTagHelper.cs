using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("easyui-textbox")]
    public class TextboxTagHelper : EasyuiTagHelper
    {
        protected override string ClassName => "easyui-textbox";
        protected override string TagName => "input";
        [HtmlAttributeName(EasyuiConsts.Editable)]
        public virtual bool? IsEdit { get; set; }

        [HtmlAttributeName(EasyuiConsts.Multiline)]
        public virtual bool? IsMultiline { get; set; }

        [HtmlAttributeName(EasyuiConsts.Disabled)]
        public virtual bool? IsDisable { get; set; }

        [HtmlAttributeName(EasyuiConsts.Required)]
        public virtual bool? IsRequired { get; set; }
        public virtual string ValidType { get; set; }
        public virtual bool? Clear { get; set; }
        public virtual int MaxLength { get; set; } = 50;

        protected List<string> ValidTypes = new List<string>();

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddIf(IsRequired.HasValue, EasyuiConsts.Required, IsRequired);
            Options.AddIf(IsMultiline.HasValue, EasyuiConsts.Multiline, IsMultiline);
            Options.AddIf(IsDisable.HasValue, EasyuiConsts.Disabled, IsDisable);
            Options.AddIf(IsEdit.HasValue, EasyuiConsts.Editable, IsEdit);

            if (MaxLength > 0)
            {
                ValidTypes.Add($"{EasyuiConsts.ValidType_MaxLength}[{MaxLength}]");
            }

            if (Options.ContainsKey(EasyuiConsts.ValidType))
            {
                var valids = Options[EasyuiConsts.ValidType].ToString().Split(',').Where(x => x.IsNotNullOrWhiteSpace());
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
                output.Attributes.Add(EasyuiConsts.Clear, Clear.ToString().ToCamelCase());

            if (ValidTypes.Any())
            {
                Options.AddOrUpdate(EasyuiConsts.ValidType, $"'{ValidTypes.JoinAsString()}'");
            }
        }
    }
}
