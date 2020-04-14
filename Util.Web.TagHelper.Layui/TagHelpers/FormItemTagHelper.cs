using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("lasyui-form-item")]
    public class FormItemTagHelper : LayuiTagHelper
    {
        protected override string ClassName => "layui-form-item";
        protected override string TagName => "div";
        public string Field { get; set; }
        public string Title { get; set; }
        public int? Sort { get; set; }
        public uint? ColSpan { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string MinWidth { get; set; }
        public TextboxTagHelper ContentTag { get; set; }
        protected override bool HasChild => false;
        public string ChildContent { get; set; }

        public uint ColMd { get; set; }

        /// <summary>
        /// 要替换的表单项的Name
        /// </summary>
        public string ReplaceField { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var hasColon = Title?.Trim() != "~";

            //var td = new StringBuilder($"<td>{Title + (hasColon ? ":" : "")}</td><td{(ColSpan.HasValue ? $" colspan='{ColSpan}'" : "")}>");
            var md = ColMd > 12 ? 12 : ColMd;

            var colspan = ColSpan > 0 ? $" {LayuiConsts.Colspan}='{ColSpan}' " : "";
            var sort = Sort.HasValue ? $" {LayuiConsts.Item_Sort}='{Sort}' " : "";
            var title = Title.IsNotNullOrWhiteSpace() ? $"<label class='layui-form-label'>{Title}</label>" : "";
            var replace = ReplaceField.IsNotNullOrWhiteSpace() ? $" {LayuiConsts.Item_Replace}='{ReplaceField}' " : "";

            if (ColSpan > 0)
            {
                md = md * ColSpan.Value;
            }

            var div = new StringBuilder($"<div class='layui-form-item layui-col-md{md}'{colspan}{sort}{replace}>{title}<div class='layui-input-block'>");

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
                div.Append(innerTagContent);
            }
            else
            {
                var content = await output.GetChildContentAsync();
                div.Append(content.GetContent());

                div.Append(ChildContent);
            }

            if (Sort.HasValue)
                output.Attributes.Add(LayuiConsts.Item_Sort, Sort);

            if (ReplaceField.IsNotNullOrWhiteSpace())
                output.Attributes.Add(LayuiConsts.Item_Replace, ReplaceField);

            div.Append(" </div> </div>");

            output.Content.AppendHtml(div.ToString());


            await base.ProcessAsync(context, output);
        }

        public static FormItemTagHelper Create(string html)
        {
            var item = new FormItemTagHelper();

            var doc = ParseHtml(html);
            var itemDiv = doc.QuerySelector(".layui-form-item");

            item.Title = itemDiv.QuerySelector(".layui-form-label")?.InnerHtml;
            item.ChildContent = itemDiv.QuerySelector(".layui-input-block").InnerHtml;
            item.Sort = itemDiv.GetAttribute(LayuiConsts.Item_Sort).ToNullable<int>();
            item.ReplaceField = itemDiv.GetAttribute(LayuiConsts.Item_Replace);
            item.ColSpan = itemDiv.GetAttribute(LayuiConsts.Colspan).ToNullable<uint>();

            return item;
        }
    }
}
