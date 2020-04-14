using System;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-datagrid-column")]
    public class DataGrodColumnTagHelper : LayuiTagHelper
    {
        protected override string ClassName => "";
        protected override string TagName => "th";
        public string Field { get; set; }
        public string Title { get; set; }
        public int? Sort { get; set; }
        public string Width { get; set; }
        public string Formatter { get; set; }
        public string Toolbar { get; set; }
        public bool Sortable { get; set; } = true;
        public Align Align { get; set; } = Align.Left;
        public string Style { get; set; }
        public Fixed? Fixed { get; set; }
        public Edit? Edit { get; set; }

        /// <summary>
        /// 要替换的列的字段名
        /// </summary>
        public string ReplaceField { get; set; }

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddOrUpdate(LayuiConsts.Grid_Col_Sortable, Sortable);
            Options.AddOrUpdate(LayuiConsts.Width, Width);
            Options.AddOrUpdate(LayuiConsts.Grid_Col_Align, Align.ToString().ToLower());
            Options.AddIf(Fixed.HasValue, LayuiConsts.Grid_Col_Fixed, Fixed.ToString().ToLower());

            Options.AddIf(Title.IsNotNullOrWhiteSpace(), LayuiConsts.Title, Title);
            Options.AddIf(Field.IsNotNullOrWhiteSpace(), LayuiConsts.Field, Field.ToCamelCase());
            Options.AddIf(Style.IsNotNullOrWhiteSpace(), LayuiConsts.Grid_Col_Style, Style);
            Options.AddIf(Formatter.IsNotNullOrWhiteSpace(), LayuiConsts.Grid_Col_Formatter, Formatter);
            Options.AddIf(Edit.HasValue, LayuiConsts.Grid_Col_Edit, Edit.ToString().ToLower());
            Options.AddIf(Toolbar.IsNotNullOrWhiteSpace(), LayuiConsts.Grid_Col_Toolbar, Toolbar);

            if (ReplaceField.IsNotNullOrWhiteSpace())
                output.Attributes.Add(LayuiConsts.Item_Replace, ReplaceField);

            base.AddOption(context, output);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.Append(Title);
            return base.ProcessAsync(context, output);
        }

        public static DataGrodColumnTagHelper Create(string html)
        {
            var col = new DataGrodColumnTagHelper();


            var th = new HtmlParser().ParseDocument($"<html> <head> </head> <body> <table> <thead>{html} </thead> </table> </body> </html>").QuerySelector("th");

            var option = th.GetAttribute(LayuiConsts.Option);
            if (option.IsNotNullOrWhiteSpace())
            {
                foreach (var x in option.Split(','))
                {
                    var items = x.Split(':');
                    col.Options.AddOrUpdate(items[0], items[1].Replace("'", ""));
                }
            }

            col.Field = col.Options.GetOrDefault(LayuiConsts.Field)?.ToString();
            col.Style = col.Options.GetOrDefault(LayuiConsts.Grid_Col_Style)?.ToString();
            col.Formatter = col.Options.GetOrDefault(LayuiConsts.Grid_Col_Formatter)?.ToString();
            col.Width = col.Options.GetOrDefault(LayuiConsts.Item_Replace)?.ToString();
            col.Sort = col.Options.GetOrDefault(LayuiConsts.Item_Sort).ToNullable<int>();
            col.Edit = col.Options.GetOrDefault(LayuiConsts.Grid_Col_Editor).ToNullable<Edit>();
            col.ReplaceField = th.GetAttribute(LayuiConsts.Item_Replace);
            col.Align = col.Options.GetOrDefault(LayuiConsts.Grid_Col_Align).To<Align>();


            return col;
        }
    }
    public enum Align
    {
        Left,
        Center,
        Right
    }
    public enum Fixed
    {
        Left,
        Right
    }

    public enum Edit
    {
        Text
    }

}
