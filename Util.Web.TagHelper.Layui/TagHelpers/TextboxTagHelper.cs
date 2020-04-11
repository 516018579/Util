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
        public virtual string ValidType { get; set; }
        public virtual bool? Clear { get; set; }
        public virtual int MaxLength { get; set; } = 50;

        protected List<string> ValidTypes = new List<string>();

        protected override void InitOption(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.AddOrUpdate(LayuiConsts.ValidAttrName, value =>
            {
                if (value != null)
                {
                    value += "|" + LayuiConsts.Required;
                }
                else
                {
                    value = LayuiConsts.Required;
                }
                return value;
            });
        }
    }
}
