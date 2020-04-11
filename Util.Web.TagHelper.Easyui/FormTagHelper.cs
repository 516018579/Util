using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Util.Application.Attributes.Easyui;
using Util.Domain;
using Util.Domain.Entities;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("eu-form")]
    public class FormTagHelper : ModelTagHelper
    {
        protected override string ClassName => "";
        protected override string TagName => "form";
        protected override bool HasChild => false;
        public int ColCount { get; set; } = 2;
        public int CellPadding { get; set; } = 5;
        public string IdName { get; set; }
        private bool ShowId => IdName.IsNotNullOrWhiteSpace();
        private int ColSpan => ColCount * 2 - 1;
        public string RemarkHeight { get; set; } = "60px";
        public bool IsSearch { get; set; } = false;

        private readonly List<FormItemTagHelper> _formItems = new List<FormItemTagHelper>();
        private readonly List<FormItemTagHelper> _textAreas = new List<FormItemTagHelper>();
        private readonly List<string> _hideItems = new List<string>();

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (HasModel)
            {
                var table = new StringBuilder($"<table cellpadding={CellPadding}>");

                CreateFormItem();

                var childHtml = (await output.GetChildContentAsync()).GetContent();
                if (childHtml.IsNotNullOrWhiteSpace())
                {
                    var childs = ParseHtml(childHtml).Body.Children;

                    while (childs.Any() && childs[0].TagName != "TR")
                    {
                        childs = childs[0].Children;
                    }

                    foreach (var child in childs)
                    {
                        var itemTag = FormItemTagHelper.Create(child.OuterHtml);

                        if (itemTag.ReplaceField.IsNotNullOrWhiteSpace())
                        {
                            var replaceTag = _formItems.FirstOrDefault(x => x.Field.Equals(itemTag.ReplaceField, StringComparison.CurrentCultureIgnoreCase));
                            if (replaceTag != null)
                            {
                                if (!itemTag.Sort.HasValue)
                                    itemTag.Sort = replaceTag.Sort;
                                itemTag.Field = replaceTag.Field;
                                if (itemTag.Title.IsNullOrWhiteSpace())
                                    itemTag.Title = replaceTag.Title;

                                itemTag.ContentTag = replaceTag.ContentTag;

                                _formItems.Remove(replaceTag);
                            }
                        }

                        _formItems.Add(itemTag);
                    }
                }

                #region 标签转html

                var index = 0;

                foreach (var item in _formItems.OrderBy(x => x.Sort))
                {
                    index++;

                    var remainder = index % ColCount;//余数

                    var td = await RenderInnerTagHelper(item, context, CreateTagHelperOutput(), false);

                    if (item.ColSpan.HasValue)
                    {
                        var itemColCount = (item.ColSpan + 1) / ColCount;

                        if (itemColCount >= (remainder == 0 ? ColCount : remainder))
                        {
                            table.Append($"<tr>{td}</tr>");
                            index += itemColCount.Value + remainder;
                            continue;
                        }
                    }

                    if (index % ColCount == 1)
                    {
                        table.Append("<tr>");
                    }

                    table.Append(td);

                    if (index > 0 && remainder == 0)
                    {
                        table.Append("</tr>");
                    }
                }

                foreach (var textArea in _textAreas)
                {
                    textArea.ColSpan = ColSpan;
                    textArea.Height = RemarkHeight;
                    textArea.MinWidth = "200px";
                    var remarkContent = await RenderInnerTagHelper(textArea, context, CreateTagHelperOutput(), false);
                    table.Append($"<tr>{remarkContent}</tr>");
                }
                #endregion

                table.Append("</table>");

                foreach (var hideItem in _hideItems)
                {
                    table.Append($"<input name=\"{hideItem.ToCamelCase()}\" hidden=\"hidden\" />");
                }

                output.Content.SetHtmlContent(table.ToString());

                await base.ProcessAsync(context, output);
            }
        }

        private void CreateFormItem()
        {
            var remarkName = nameof(IHasRemark.Remark);
            var index = 0;
            foreach (var property in PropertyInfos)
            {
                var name = property.Name;
                var isId = name == nameof(WebConsts.Id);
                var propertyType = property.PropertyType;
                if (property.IsDefined(typeof(NotFormItemAttribute), false))
                    continue;
                if (isId && !ShowId)
                    continue;
                var isFileType = propertyType == typeof(IFormFile);
                if (!propertyType.IsValueType() && !isFileType)
                    continue;
                if (property.IsDefined(typeof(HideFormItemAttribute), false))
                {
                    _hideItems.Add(property.Name);
                    continue;
                }
                if (name == remarkName && IsRemarkType)
                {
                    var remarkTag = new TextboxTagHelper();
                    remarkTag.IsMultiline = true;
                    remarkTag.MaxLength = DomainConsts.MaxDescLength;
                    _textAreas.Add(CreateItemTag(remarkName, WebConsts.DisplayName_Remark, remarkTag));
                    continue;
                }

                index++;


                var valueType = propertyType.GetValueType();
                var isNullType = propertyType.IsNullableType();
                var isValueType = propertyType.IsValueType && !isNullType;
                var isName = IsNameType && name == nameof(IHasName.Name);
                var attributes = property.GetCustomAttributes(true);
                var isDisable = GetAttribute<DisableAttribute>(attributes) != null;
                var colNameAttr = GetAttribute<DisplayNameAttribute>(attributes);
                var requiredAttr = GetAttribute<RequiredAttribute>(attributes);
                var comboboxAttr = GetAttribute<ComboboxAttribute>(attributes);

                var isCombo = comboboxAttr != null || propertyType == typeof(bool) || propertyType == typeof(bool?) || valueType.IsEnum;
                var colName = colNameAttr == null ? name : colNameAttr.DisplayName;

                if (name == colName)
                {
                    if (isFileType)
                        colName = "文件";
                    else
                    {
                        switch (name)
                        {
                            case nameof(IHasRemark.Remark):
                                colName = WebConsts.DisplayName_Remark;
                                break;
                            case nameof(IHasName.Name):
                                colName = WebConsts.DisplayName_Name;
                                break;
                            case nameof(WebConsts.IsActive):
                                colName = WebConsts.IsAble_Name;
                                break;
                            case nameof(IHasSort.Sort):
                                colName = WebConsts.Sort_Name;
                                break;
                            case nameof(WebConsts.Id):
                                colName = IdName;
                                break;
                            default:
                                if (valueType.IsEnum)
                                    colName = valueType.GetDisplayName();
                                break;
                        }
                    }
                }

                var isTextArea = false;
                var isRequired = isId || (isName && !IsSearch) || isValueType || requiredAttr != null;
                int maxLength = DomainConsts.DefaultStringLength;

                TextboxTagHelper itemTag = null;


                if (isCombo)
                {
                    var defaultValueAttr = GetAttribute<DefaultValueAttribute>(attributes);
                    var defaultIndexAttr = GetAttribute<DefaultIndexAttribute>(attributes);

                    var isEnum = valueType.IsEnum;
                    var tag = new ComboboxTagHelper
                    {
                        DefaultValue = defaultValueAttr?.Value,
                        DefaultIndex = defaultIndexAttr?.Index
                    };

                    if (isEnum)
                    {
                        tag.ModelType = valueType;
                    }
                    else if (comboboxAttr != null)
                    {
                        if (comboboxAttr.IsLoadData)
                        {
                            tag.ModelType = comboboxAttr.Type;
                        }

                        if (colNameAttr == null && comboboxAttr.DisplayName != null)
                        {
                            colName = comboboxAttr.DisplayName;
                        }
                    }
                    else if (valueType == typeof(bool))
                    {
                        tag.Class.Add("boolCombobox");
                    }

                    itemTag = tag;
                }
                else if (propertyType.IsNumberType())
                {
                    var precisionAttr = GetAttribute<PrecisionAttribute>(attributes);
                    itemTag = new NumberboxTagHelper
                    {
                        Precision = precisionAttr?.Precision
                    };
                }
                else if (valueType == typeof(DateTime))
                {
                    var datetimeAttr = GetAttribute<DateTimeBoxAttribute>(attributes);
                    if (datetimeAttr == null)
                        itemTag = new DateboxTagHelper();
                    else
                        itemTag = new DateTimeboxTagHelper();
                }
                else if (isFileType)
                {
                    itemTag = new FileTagHelper();
                }
                else
                {
                    var maxLengthAttr = GetAttribute<MaxLengthAttribute>(attributes);
                    isTextArea = GetAttribute<TextAreaAttribute>(attributes) != null;

                    var tag = new TextboxTagHelper();
                    if (maxLengthAttr != null)
                        maxLength = maxLengthAttr.Length;

                    var stringLength = GetAttribute<StringLengthAttribute>(attributes)?.MaximumLength;
                    if (stringLength.HasValue)
                    {
                        maxLength = stringLength.Value;
                    }

                    if (isTextArea)
                    {
                        tag.IsMultiline = true;
                        if (maxLengthAttr == null)
                            maxLength = DomainConsts.MaxDescLength;
                    }

                    itemTag = tag;
                }

                if (isDisable)
                {
                    itemTag.IsDisable = isDisable;
                }

                itemTag.IsRequired = isRequired;
                itemTag.MaxLength = maxLength;

                if (isTextArea)
                    _textAreas.Add(CreateItemTag(name, colName, itemTag, index));
                else
                    _formItems.Add(CreateItemTag(name, colName, itemTag, isId ? 0 : index));
            }
        }

        private T GetAttribute<T>(IEnumerable<object> attributes) where T : Attribute
        {
            return attributes.OfType<T>().FirstOrDefault(x => x is T);
        }

        private FormItemTagHelper CreateItemTag(string itemName, string itemTitle, TextboxTagHelper contentTag, int itemSort = int.MaxValue)
        {
            var tag = new FormItemTagHelper();
            tag.Field = itemName;
            tag.Title = itemTitle;
            tag.Sort = itemSort;
            tag.ContentTag = contentTag;
            return tag;
        }
    }
}
