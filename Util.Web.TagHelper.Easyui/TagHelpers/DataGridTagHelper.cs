using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Application.Attributes.Control;
using Util.Application.Attributes.Format;
using Util.Domain;
using Util.Domain.Entities;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("easyui-datagrid")]
    public class DataGridTagHelper : EasyuiModelTagHelper
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

        private readonly List<DataGrodColumnTagHelper> _cols = new List<DataGrodColumnTagHelper>();

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.Add(EasyuiConsts.Page_List, GetJavaScriptString(PageList));
            Options.Add(EasyuiConsts.Page_Size, PageSize);
            Options.Add(EasyuiConsts.Sort_Name, SortName);
            Options.Add(EasyuiConsts.Sort_Order, SortOrder);
            Options.Add(EasyuiConsts.ShowRowNumber, ShowRowNumber);
            Options.Add(EasyuiConsts.ShowPage, ShowPage);
            Options.Add(EasyuiConsts.SingleSelect, SingleSelect);
            Options.Add(EasyuiConsts.ShowHeader, ShowHeader);
            Options.Add(EasyuiConsts.Striped, Striped);

            Options.AddIf(ToolBar.IsNotNullOrWhiteSpace(), EasyuiConsts.ToolBar, ToolBar);
            Options.AddIf(Url.IsNotNullOrWhiteSpace(), EasyuiConsts.Url, Url);
            Options.AddIf(Data.IsNotNullOrWhiteSpace(), EasyuiConsts.Data, GetJavaScriptString(Data));
            Options.AddIf(QueryParams.IsNotNullOrWhiteSpace(), EasyuiConsts.QueryParams, GetJavaScriptString(QueryParams));

            output.Attributes.Add(EasyuiConsts.Fit, Fit.ToString().ToLower());

            base.AddOption(context, output);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (ModelTagHelper.HasModel)
            {
                var table = new StringBuilder();

                DefaultWidth = DefaultWidth ?? 200;

                foreach (var property in ModelTagHelper.PropertyInfos)
                {
                    var isId = property.Name == WebConsts.Id;
                    if (property.IsDefined(typeof(ColHideAttribute)))
                        continue;
                    if (property.IsDefined(typeof(ComboboxAttribute)))
                        continue;
                    if (isId && IdName.IsNullOrWhiteSpace())
                        continue;


                    if (property.PropertyType.IsGenericType && property.PropertyType.IsAssignableFromGenericTypeInterface(typeof(IEnumerable<>)))
                    {
                        var genericTypes = property.PropertyType.GetGenericArguments();
                        if (genericTypes.Length > 1 || !genericTypes[0].IsValueType())
                        {
                            continue;
                        }
                    }

                    var col = new DataGrodColumnTagHelper();


                    col.IsFrozen = property.IsDefined(typeof(FixedColAttribute));

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
                            data = EnumUtil.BoolDictionary.Select(x => new { value = x.Key, text = x.Value })
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

                    var childs = HtmlTrim(childHtml).Split(split, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var child in childs)
                    {
                        var itemTag = DataGrodColumnTagHelper.Create(child + split);
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

                if (!FitColumns.HasValue)
                {
                    if (ColCount < FitColCount)
                    {
                        FitColumns = true;
                    }
                    else
                    {
                        FitColumns = FitColCount > ColCount;
                    }
                }


                Options.Add(EasyuiConsts.FitColumns, FitColumns);

                var checkBox = "<th data-options=\"field:'_ck',checkbox: true\"></th>";

                var frozenCols = _cols.Where(x => x.IsFrozen).ToList();

                async Task AddHead(List<DataGrodColumnTagHelper> cols, bool isFrozen = false)
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
