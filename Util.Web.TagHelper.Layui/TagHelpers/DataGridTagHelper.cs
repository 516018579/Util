using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Application.Attributes.Format;
using Util.Domain;
using Util.Domain.Entities;
using Util.Extensions;
using Util.Json;
using Util.Web.Attributes.Control;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-datagrid")]
    public class DataGridTagHelper : LayuiModelTagHelper
    {
        protected override string ClassName => "layui-table";
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
        public bool Page { get; set; } = true;
        public bool HasCheckBox { get; set; } = true;
        public bool IsSort { get; set; } = true;
        public bool ShowHeader { get; set; } = true;
        /// <summary>
        /// 是否开启隔行背景
        /// </summary>
        public bool Striped { get; set; } = false;
        public string ToolBar { get; set; }
        public string IdName { get; set; }
        public string DefaultWidth { get; set; }
        public Size? Size { get; set; }
        public Skin? Skin { get; set; }
        private int ColCount => _cols.Count;


        protected override bool HasChild => false;

        /// <summary>
        /// 当列数小于此数量是设置自动宽度
        /// </summary>
        public int FitColCount { get; set; } = 6;

        private readonly List<DataGrodColumnTagHelper> _cols = new List<DataGrodColumnTagHelper>();

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.Add(LayuiConsts.Grid_PageList, GetJavaScriptString(PageList));
            Options.Add(LayuiConsts.Grid_PageSize, PageSize);
            Options.Add(LayuiConsts.Gird_Page, Page);
            Options.Add(LayuiConsts.ShowHeader, ShowHeader);
            Options.Add(LayuiConsts.Striped, Striped);

            Options.AddIf(Size.HasValue, LayuiConsts.Grid_Size, Size.ToString().ToLower());
            Options.AddIf(Skin.HasValue, LayuiConsts.Grid_Skin, Skin.ToString().ToLower());
            Options.AddIf(ToolBar.IsNotNullOrWhiteSpace(), LayuiConsts.Grid_ToolBar, ToolBar);
            Options.AddIf(Url.IsNotNullOrWhiteSpace(), LayuiConsts.Url, Url);
            Options.AddIf(Data.IsNotNullOrWhiteSpace(), LayuiConsts.Data, GetJavaScriptString(Data));
            Options.AddIf(QueryParams.IsNotNullOrWhiteSpace(), LayuiConsts.QueryParams, GetJavaScriptString(QueryParams));
            Options.AddIf(LayuiTagHelperConfig.GridParseData.IsNotNullOrWhiteSpace(), LayuiConsts.Grid_ParseData, GetJavaScriptString(LayuiTagHelperConfig.GridParseData));
            Options.AddIf(LayuiTagHelperConfig.GridDefaultParam.IsNotNullOrWhiteSpace(), LayuiConsts.Grid_DefaultParam, GetJavaScriptString(LayuiTagHelperConfig.GridDefaultParam));

            Options.Add(LayuiConsts.Grid_Sort, new Dictionary<string, object> { { LayuiConsts.Sort_Name, SortName }, { LayuiConsts.Sort_Order, SortOrder } });

            output.Attributes.Add(LayuiConsts.Fit, Fit.ToString().ToLower());

            base.AddOption(context, output);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (ModelTagHelper.HasModel)
            {
                var table = new StringBuilder();

                foreach (var property in ModelTagHelper.PropertyInfos)
                {
                    var isId = property.Name == WebConsts.Id;
                    if (property.IsDefined(typeof(ColHideAttribute)))
                        continue;
                    if (property.IsDefined(typeof(ComboboxAttribute)))
                        continue;
                    if (isId && IdName.IsNullOrWhiteSpace())
                        continue;

                    var col = new DataGrodColumnTagHelper();


                    var fixedAttr = property.GetCustomAttribute<FixedColAttribute>();
                    if (fixedAttr?.Fixed != null)
                    {
                        col.Fixed = fixedAttr.Fixed.To<Fixed>();
                    }


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

                        //if (col.Editor.IsNullOrWhiteSpace())
                        //    col.Editor = $"{{ type: 'combobox', options: {{ valueField: 'value', textField: 'text', data: {data}, required: {propertyType.IsValueType().ToString().ToLower()} }} }}";
                    }
                    else
                    {
                        if (propertyType.IsNumberType())
                        {
                            //col.Edit = "numberbox";
                        }
                        else if (valueType == typeof(DateTime))
                        {
                            //col.Edit = "datebox";
                        }
                        else
                        {
                            col.Edit = Edit.Text;
                        }
                    }

                    col.Field = property.Name;
                    col.Width = DefaultWidth;
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
                                itemTag.Width = replaceTag.Width;
                                itemTag.Style = replaceTag.Style;
                                itemTag.Title = replaceTag.Title;
                                itemTag.Formatter = replaceTag.Formatter;
                                if (!itemTag.Sort.HasValue)
                                    itemTag.Sort = replaceTag.Sort;
                                if (itemTag.Edit.HasValue)
                                {
                                    itemTag.Edit = replaceTag.Edit;
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

                Options.Add(LayuiConsts.FitColumns, FitColumns);

                async Task AddHead(List<DataGrodColumnTagHelper> cols)
                {
                    table.Append("<thead>");
                    if (HasCheckBox)
                        table.Append($"<th {LayuiConsts.Option}=\"{{ type: 'checkbox' , fixed: 'left'}}\"></th>");
                    if (ShowRowNumber)
                        table.Append($"<th {LayuiConsts.Option}=\"{{ type: 'numbers' , fixed: 'left'}} \"></th>");


                    foreach (var col in cols.OrderBy(x => x.Sort))
                    {
                        table.Append(await RenderInnerTagHelper(col, context, CreateTagHelperOutput()));
                    }

                    table.Append("</thead>");
                }


                await AddHead(_cols.ToList());

                output.Content.SetHtmlContent(table.ToString());
            }
            await base.ProcessAsync(context, output);
        }


    }
    public enum Size
    {
        Sm,
        Lg
    }

    public enum Skin
    {
        Line,
        Row,
        Nob
    }
}
