using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Domain;
using Util.Json;
using Util.Web.TagHelpers;
using Util.Web.TagHelpers.Easyui;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("easyui-combobox")]
    public class ComboboxTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-combobox";
        protected override string TagName => "select";
        public Type ModelType { get; set; }
        [HtmlAttributeName("data")]
        public string Data { get; set; }
        [HtmlAttributeName("default-value")]
        public dynamic DefaultValue { get; set; }
        [HtmlAttributeName("default-index")]
        public int? DefaultIndex { get; set; }
        [HtmlAttributeName(EasyuiConsts.Editable)]
        public override bool? IsEdit { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Dictionary<string, string> list;

            if (ModelType == null)
            {
                list = Data.ToEnumerable<Dictionary<string, string>>();
            }
            else if (ModelType.IsEnum)
            {
                list = EnumUtil.GetEnumValueList(ModelType).ToDictionary(x => x.Key.ToString(), x => x.Value);
            }
            else
            {
                list = await TagHelperConfig.GetComboboxDataFunc(ModelType.Name);
            }

            if (DefaultValue != null && DefaultValue is bool)
            {
                string value = DefaultValue.ToString();
                output.Attributes.Add("defaultValue", value.ToCamelCase());
            }

            if (DefaultIndex >= 0)
            {
                output.Attributes.Add("defaultIndex", DefaultIndex);
            }

            int i = 0;
            foreach (var item in list)
            {
                if (DefaultValue != null && DefaultValue.GetType().IsEnum)
                    DefaultValue = (int)DefaultValue;

                var isSelected = item.Value == DefaultValue || i == DefaultIndex;

                output.Content.AppendHtml($"<option value='{item.Key}' {(isSelected ? "selected='selected'" : "")}>{item.Value}</option>");
                i++;
            }



            await base.ProcessAsync(context, output);
        }
    }
}
