using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;
using Util.Web.TagHelpers.Extensions;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-textbox")]
    public class TextboxTagHelper : LayuiTagHelper
    {
        protected override string ClassName => "layui-input";
        protected override string TagName => "input";
        [HtmlAttributeName(LayuiConsts.Editable)]
        public virtual bool? IsEdit { get; set; }

        [HtmlAttributeName(LayuiConsts.Disabled)]
        public virtual bool? IsDisable { get; set; }

        [HtmlAttributeName(LayuiConsts.Required)]
        public virtual bool? IsRequired { get; set; }
        public virtual bool? Clear { get; set; }
        public virtual int MaxLength { get; set; } = 50;

        protected virtual HashSet<string> Valids { get; set; } = new HashSet<string>();


        protected override void InitOption(TagHelperContext context, TagHelperOutput output)
        {
            InitValids(output);
            base.InitOption(context, output);
        }

        protected virtual void InitValids(TagHelperOutput output)
        {
            if (output.Attributes.ContainsName(LayuiConsts.ValidAttrName))
            {
                var vaild = output.Attributes[LayuiConsts.ValidAttrName]?.Value?.ToString();

                if (vaild.IsNotNullOrWhiteSpace())
                {
                    foreach (var item in vaild.Split('|', StringSplitOptions.RemoveEmptyEntries))
                    {
                        Valids.Add(item);
                    }
                }
            }

            if (IsRequired == true)
            {
                Valids.Add(LayuiConsts.Required);
            }

            output.Attributes.AddOrUpdate(LayuiConsts.ValidAttrName, Valids.JoinAsString('|'));
        }
    }
}
