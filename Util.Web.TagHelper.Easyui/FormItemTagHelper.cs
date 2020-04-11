using System;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Util.Extensions;
using Util.Web;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("eu-form-item")]
    public class FormItemTagHelper : EasyuiTagHelper
    {
        protected override string ClassName => "";
        protected override string TagName => "tr";
        public string Field { get; set; }
        public string Title { get; set; }
        public int? Sort { get; set; }
        public int? ColSpan { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string MinWidth { get; set; }
        public TextboxTagHelper ContentTag { get; set; }
        protected override bool HasChild => false;
        public string ChildContent { get; set; }

        /// <summary>
        /// 要替换的表单项的Name
        /// </summary>
        public string ReplaceField { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var hasColon = Title?.Trim() != "~";
            var td = new StringBuilder($"<td>{Title + (hasColon ? ":" : "")}</td><td{(ColSpan.HasValue ? $" colspan='{ColSpan}'" : "")}>");

            if (ContentTag != null)
            {
                var innerOutput = CreateTagHelperOutput();
                if (Field.IsNotNullOrWhiteSpace())
                    innerOutput.Attributes.Add("name", Field.ToCamelCase());

                var css = "";
                if (Height.IsNotNullOrWhiteSpace())
                    css += $"height: {Height};";
                if (Width.IsNotNullOrWhiteSpace())
                    css += $"width: {Width};";
                if (MinWidth.IsNotNullOrWhiteSpace())
                    css += $"min-width: {MinWidth};";
                if (ColSpan > 1)
                    css += "width: 100%;";
                if (css != "")
                    innerOutput.Attributes.Add("style", css);

                var innerTagContent = await RenderInnerTagHelper(ContentTag, context, innerOutput);
                td.Append(innerTagContent);
            }
            else
            {
                var content = await output.GetChildContentAsync();
                td.Append(content.GetContent());

                td.Append(ChildContent);
            }

            if (Sort.HasValue)
                output.Attributes.Add(WebConsts.Easyui.Item_Sort, Sort);

            if (ReplaceField.IsNotNullOrWhiteSpace())
                output.Attributes.Add(WebConsts.Easyui.Item_Replace, ReplaceField);

            td.Append("</td>");

            output.Content.AppendHtml(td.ToString());


            await base.ProcessAsync(context, output);
        }

        public static FormItemTagHelper Create(string html)
        {
            var item = new FormItemTagHelper();

            var tr = ParseHtml(html).QuerySelector("tr");
            var tds = tr.QuerySelectorAll("td");

            if (tds.Any())
            {
                item.Title = tds[0].InnerHtml.TrimEnd(':');

                var td = tds[1];

                var colSpan = td.GetAttribute("colspan");
                if (colSpan.IsNotNullOrWhiteSpace())
                {
                    item.ColSpan = colSpan.To<int>();
                }

                item.ChildContent = td.InnerHtml;
                item.Sort = tr.GetAttribute(WebConsts.Easyui.Item_Sort).ToNullable<int>();
                item.ReplaceField = tr.GetAttribute(WebConsts.Easyui.Item_Replace);
            }


            return item;
        }
    }
}
