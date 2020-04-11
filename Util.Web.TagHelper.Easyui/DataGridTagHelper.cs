using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Util.Application.Attributes.Easyui;
using Util.Application.Attributes.Format;
using Util.Domain;
using Util.Domain.Entities;
using Util.Extensions;
using Util.Json;
using ColHideAttribute = Util.Web.Attributes.Control.ColHideAttribute;
using ComboboxAttribute = Util.Web.Attributes.Control.ComboboxAttribute;
using FrozenColAttribute = Util.Web.Attributes.Control.FrozenColAttribute;
using IsAbleFormatAttribute = Util.Web.Attributes.Format.IsAbleFormatAttribute;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("datagrid")]
    public class DataGridTagHelper : ModelTagHelper
    {
        protected override string ClassName => "easyui-datagrid";
        protected override string TagName => "table";
        public string PageList { get; set; } = "[20, 100, 500, 1000]";
        public int PageSize { get; set; } = 20;
        public string SortName { get; set; } = WebConsts.Id;
        public string SortOrder { get; set; } = WebConsts.Order_Asc;
        public string Url { get; set; }
        public string QueryParams { get; set; }
        public string Data { get; set; }
        public bool? FitColumns { get; set; }
        public bool Fit { get; set; } = true;
        public bool ShowRowNumber { get; set; } = true;
        public bool ShowPage { get; set; } = true;
        public bool HasCheckBox { get; set; } = true;
        public bool IsSort { get; set; } = true;
        public bool SingleSelect { get; set; }
        public bool ShowHeader { get; set; } = true;
        public bool Striped { get; set; } = true;
        public string ToolBar { get; set; }
        public string IdName { get; set; }
        public int? DefaultWidth { get; set; } = 200;
        private int ColCount => _cols.Count;
        protected override bool HasChild => false;

        /// <summary>
        /// 当列数小于此数量是设置自动宽度
        /// </summary>
        public int FitColCount { get; set; } = 6;

        private readonly List<DataGrodColTagHelper> _cols = new List<DataGrodColTagHelper>();

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.Add(WebConsts.Easyui.Page_List, PageList);
            Options.Add(WebConsts.Easyui.Page_Size, PageSize);
            Options.Add(WebConsts.Easyui.Sort_Name, SortName.ToJsonString());
            Options.Add(WebConsts.Easyui.Sort_Order, SortOrder.ToJsonString());
            Options.Add(WebConsts.Easyui.ShowRowNumber, ShowRowNumber);
            Options.Add(WebConsts.Easyui.ShowPage, ShowPage);
            Options.Add(WebConsts.Easyui.SingleSelect, SingleSelect);
            Options.Add(WebConsts.Easyui.ShowHeader, ShowHeader);
            Options.Add(WebConsts.Easyui.Striped, Striped);

            Options.AddIf(ToolBar.IsNotNullOrWhiteSpace(), WebConsts.Easyui.ToolBar, ToolBar.ToJsonString());
            Options.AddIf(Url.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Url, Url.ToJsonString());
            Options.AddIf(Data.IsNotNullOrWhiteSpace(), WebConsts.Easyui.Data, Data);
            Options.AddIf(QueryParams.IsNotNullOrWhiteSpace(), WebConsts.Easyui.QueryParams, QueryParams);

            output.Attributes.Add(WebConsts.Easyui.Fit, Fit.ToString().ToLower());

            base.AddOption(context, output);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (HasModel)
            {
                var table = new StringBuilder();

                DefaultWidth = DefaultWidth ?? 200;

                foreach (var property in PropertyInfos)
                {
                    var isId = property.Name == WebConsts.Id;
                    if (property.IsDefined(typeof(ColHideAttribute)))
                        continue;
                    if (property.IsDefined(typeof(ComboboxAttribute)))
                        continue;
                    if (isId && IdName.IsNullOrWhiteSpace())
                        continue;

                    var col = new DataGrodColTagHelper();


                    col.IsFrozen = property.IsDefined(typeof(FrozenColAttribute));

                    var propertyType = property.PropertyType;
                    var valueType = propertyType.GetValueType();
                    var isEnum = valueType.IsEnum;
                    var name = property.GetDisplayName();
                    var isBool = valueType == typeof(bool);
                    if (name == property.Name)
                    {
                        switch (name)
                        {
                            case nameof(IHasRemark.Remark):
                                name = WebConsts.DisplayName_Remark;
                                break;
                            case nameof(IHasName.Name):
                                name = WebConsts.DisplayName_Name;
                                break;
                            case nameof(WebConsts.IsActive):
                                name = WebConsts.IsAble_Name;
                                break;
                            case nameof(IHasSort.Sort):
                                name = WebConsts.Sort_Name;
                                break;
                            case WebConsts.Id:
                                name = IdName;
                                break;
                            default:
                                if (isEnum)
                                    name = valueType.GetDisplayName();
                                break;
                        }
                    }

                    if (isEnum || isBool)
                    {
                        string data;
                        if (isBool)
                        {
                            data = (property.GetCustomAttribute<IsAbleFormatAttribute>() != null
                                    ? EnumUtil.IsAbleDictionary
                                    : EnumUtil.BoolDictionary).Select(x => new { value = x.Key, text = x.Value })
                                .ToJsonString(true);
                        }
                        else
                        {
                            data = EnumUtil.GetEnumValueList(valueType)
                                .Select(x => new { value = x.Key, text = x.Value.Replace("\"", "&quot;") })
                                .ToJsonString(true);
                        }


                        if (col.Formatter.IsNullOrWhiteSpace())
                            col.Formatter = $"function(value, rowData, rowIndex) {{ return getArrayText({data}, value, 'text', 'value'); }}";

                        if (col.Editor.IsNullOrWhiteSpace())
                            col.Editor = $"{{ type: 'combobox', options: {{ valueField: 'value', textField: 'text', data: {data}, required: {propertyType.IsValueType().ToString().ToLower()} }} }}";
                    }
                    else
                    {
                        if (propertyType.IsNumberType())
                        {
                            col.Editor = "numberbox";
                        }
                        else if (valueType == typeof(DateTime))
                        {
                            col.Editor = "datebox";
                        }
                        else
                        {
                            col.Editor = "text";
                        }

                        col.Editor = $"'{col.Editor}'";
                    }

                    col.Field = property.Name;
                    col.Width = DefaultWidth.Value;
                    col.Sortable = IsSort;
                    col.Title = name;

                    _cols.Add(col);
                    col.Sort = isId ? 0 : ColCount;
                }

                var childHtml = (await output.GetChildContentAsync()).GetContent();
                if (childHtml.IsNotNullOrWhiteSpace())
                {
                    var split = "</th>";

                    var childs = HmlTrim(childHtml).Split(split, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var child in childs)
                    {
                        var itemTag = DataGrodColTagHelper.Create(child + split);
                        if (itemTag.ReplaceField.IsNotNullOrWhiteSpace())
                        {
                            var replaceTag = _cols.FirstOrDefault(x => x.Field.Equals(itemTag.ReplaceField, StringComparison.CurrentCultureIgnoreCase));
                            if (replaceTag != null)
                            {
                                itemTag.Field = replaceTag.Field;
                                if (itemTag.Title.IsNullOrWhiteSpace())
                                    itemTag.Title = replaceTag.Title;
                                if (!itemTag.Sort.HasValue)
                                    itemTag.Sort = replaceTag.Sort;
                                if (itemTag.Editor.IsNullOrWhiteSpace())
                                {
                                    itemTag.Editor = replaceTag.Editor;
                                }
                                if (itemTag.Formatter.IsNullOrWhiteSpace())
                                {
                                    itemTag.Formatter = replaceTag.Formatter;
                                }
                                if (itemTag.Styler.IsNullOrWhiteSpace())
                                {
                                    itemTag.Styler = replaceTag.Styler;
                                }
                                if (itemTag.Width == 0)
                                {
                                    itemTag.Width = replaceTag.Width;
                                }

                                _cols.Remove(replaceTag);
                            }
                        }

                        _cols.Add(itemTag);
                    }
                }


                if (!FitColumns.HasValue && ColCount < FitColCount)
                    FitColumns = true;
                else
                    FitColumns = FitColCount > ColCount;

                Options.Add(WebConsts.Easyui.FitColumns, FitColumns);

                var checkBox = "<th data-options=\"field:'_ck',checkbox: true\"></th>";

                var frozenCols = _cols.Where(x => x.IsFrozen).ToList();

                async Task AddHead(List<DataGrodColTagHelper> cols, bool isFrozen = false)
                {
                    table.Append($"<thead {(isFrozen ? "data-options='frozen: true'" : "")}>");
                    if (HasCheckBox && (isFrozen || (!isFrozen && !frozenCols.Any())))
                        table.Append(checkBox);

                    foreach (var col in cols.OrderBy(x => x.Sort))
                    {
                        if (!col.IsEdit)
                            col.Editor = null;
                        table.Append(await RenderInnerTagHelper(col, context, CreateTagHelperOutput()));
                    }

                    table.Append("</thead>");
                }

                if (frozenCols.Any())
                {
                    await AddHead(frozenCols, true);
                }

                await AddHead(_cols.Where(x => !x.IsFrozen).ToList());

                output.Content.SetHtmlContent(table.ToString());
            }
            await base.ProcessAsync(context, output);
        }
    }
}
