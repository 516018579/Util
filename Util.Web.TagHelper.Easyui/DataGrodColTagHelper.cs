using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Util.Extensions;
using Util.Web;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("datagrid-col")]
    public class DataGrodColTagHelper : EasyuiTagHelper
    {
        protected override string ClassName => "";
        protected override string TagName => "th";
        public string Field { get; set; }
        public string Title { get; set; }
        public int? Sort { get; set; }
        public int Width { get; set; }
        public string Formatter { get; set; }
        public bool Sortable { get; set; } = true;
        public Align Align { get; set; } = Align.Left;
        public string Styler { get; set; }
        public string Editor { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsEdit { get; set; }

        /// <summary>
        /// 要替换的列的字段名
        /// </summary>
        public string ReplaceField { get; set; }

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.AddOrUpdate(WebConsts.Easyui.Grid_Col_Align, $"'{Align.ToString().ToLower()}'");
            Options.AddIf(Field.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Field, $"'{Field?.ToCamelCase()}'");
            Options.AddIf(Title.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Title, $"'{Title}'");
            Options.AddOrUpdate(WebConsts.Easyui.Width, Width);
            Options.AddIf(Styler.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Grid_Col_Styler, Styler);
            Options.AddIf(Formatter.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Grid_Col_Formatter, Formatter);
            Options.AddIf(Editor.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Grid_Col_Editor, Editor);

            if (IsFrozen)
                output.Attributes.Add(WebConsts.Easyui.Grid_Col_IsFrozen, IsFrozen);

            if (IsEdit)
                output.Attributes.Add(WebConsts.Easyui.Grid_Col_IsEdit, IsEdit);

            if (ReplaceField.IsNotNullOrWhiteSpace())
                output.Attributes.Add(WebConsts.Easyui.Item_Replace, ReplaceField);

            base.AddOption(context, output);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.Append(Title);
            return base.ProcessAsync(context, output);
        }

        public static DataGrodColTagHelper Create(string html)
        {
            var col = new DataGrodColTagHelper();


            var th = new HtmlParser().ParseDocument($"<html> <head> </head> <body> <table> <thead>{html} </thead> </table> </body> </html>").QuerySelector("th");

            var option = th.GetAttribute(WebConsts.Easyui.Option);
            if (option.IsNotNullOrWhiteSpace())
            {
                foreach (var x in option.Split(','))
                {
                    var items = x.Split(':');
                    col.Options.AddOrUpdate(items[0], items[1].Replace("'", ""));
                }
            }

            col.Field = col.Options.GetOrDefault(WebConsts.Easyui.Field)?.ToString();
            col.Styler = col.Options.GetOrDefault(WebConsts.Easyui.Grid_Col_Styler)?.ToString();
            col.Editor = col.Options.GetOrDefault(WebConsts.Easyui.Grid_Col_Editor)?.ToString();
            col.Formatter = col.Options.GetOrDefault(WebConsts.Easyui.Grid_Col_Formatter)?.ToString();
            col.Width = col.Options.GetOrDefault(WebConsts.Easyui.Item_Replace).To(0);
            col.Sort = col.Options.GetOrDefault(WebConsts.Easyui.Item_Sort).ToNullable<int>();

            col.ReplaceField = th.GetAttribute(WebConsts.Easyui.Item_Replace);
            col.IsFrozen = th.GetAttribute(WebConsts.Easyui.Grid_Col_IsFrozen).To(false);
            col.IsEdit = th.GetAttribute(WebConsts.Easyui.Grid_Col_IsEdit).To(false);

            var align = col.Options.GetOrDefault(WebConsts.Easyui.Grid_Col_Align)?.ToString();
            if (align.IsNotNullOrWhiteSpace())
            {
                switch (align)
                {
                    case "center":
                        col.Align = Align.Center;
                        break;
                    case "right":
                        col.Align = Align.Right;
                        break;
                    default:
                        col.Align = Align.Left;
                        break;
                }
            }

            return col;
        }
    }
    public enum Align
    {
        Left,
        Center,
        Right
    }
}
