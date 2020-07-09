using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Application.Attributes.Control;
using Util.Application.Attributes.Format;
using Util.Domain;
using Util.Domain.Attributes;
using Util.Domain.Entities;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    [HtmlTargetElement("easyui-form")]
    public class FormTagHelper : EasyuiModelTagHelper
    {
        protected override string ClassName => "";
        protected override string TagName => "form";
        protected override bool HasChild => false;
        public uint ColCount { get; set; } = 2;
        public uint CellPadding { get; set; } = 5;
        public string IdName { get; set; }
        public bool HasId { get; set; }
        private uint ColSpan => ColCount * 2 - 1;
        public string RemarkHeight { get; set; } = "60px";
        public bool IsSearch { get; set; } = false;
        public uint? ItemWidth { get; set; }

        private readonly List<FormItemTagHelper> _formItems = new List<FormItemTagHelper>();
        private readonly List<FormItemTagHelper> _textAreas = new List<FormItemTagHelper>();
        private readonly List<string> _hideItems = new List<string>();
        private readonly HashSet<string> _item = new HashSet<string>();

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
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

                            //itemTag.ContentTag = replaceTag.ContentTag;
                            _formItems.Remove(replaceTag);
                        }
                    }

                    _formItems.Add(itemTag);
                }
            }

            #region 标签转html

            uint index = 0;

            foreach (var item in _formItems.OrderBy(x => x.Sort))
            {
                if (_item.Contains(item.Field))
                    continue;
                index++;

                var remainder = index % ColCount;//余数

                StringBuilder td = new StringBuilder();
                if (item.Group.IsNotNullOrWhiteSpace())
                {
                    var groupIndex = 0;

                    foreach (var groupItem in _formItems.Where(x => x.Group == item.Group))
                    {
                        var itemContent = await RenderInnerTagHelper(groupItem, context, CreateTagHelperOutput(), false);
                        var doc = new HtmlParser().ParseDocument($"<table><tr>{itemContent}</tr></table>");
                        var items = doc.QuerySelectorAll("td");

                        if (groupIndex == 0)
                        {
                            td.Append(items[0].OuterHtml);
                            td.Append($"<td {(groupItem.ColSpan.HasValue ? $"colspan='{groupItem.ColSpan}'" : "")}>");
                        }

                        td.Append($"{groupItem.Before}{items[1].InnerHtml}{groupItem.After}");
                        _item.Add(groupItem.Field);

                        groupIndex++;
                    }
                    td.Append("</td>");
                }
                else
                    td.Append(await RenderInnerTagHelper(item, context, CreateTagHelperOutput(), false));


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

                if (index > 0 && remainder == 0 || _formItems.Count == 1)
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

        private void CreateFormItem()
        {
            if (ModelTagHelper.HasModel)
            {
                var remarkName = nameof(IHasRemark.Remark);
                var index = 0;
                foreach (var property in ModelTagHelper.PropertyInfos)
                {
                    var name = property.Name;
                    var isId = name == nameof(WebConsts.Id);
                    var propertyType = property.PropertyType;
                    if (property.IsDefined(typeof(NotFormItemAttribute), false))
                        continue;
                    var isFileType = propertyType == typeof(IFormFile);
                    if (!propertyType.IsValueType() && !isFileType)
                        continue;
                    if (isId && !HasId)
                    {
                        continue;
                    }
                    if (property.IsDefined(typeof(HideFormItemAttribute), false) || isId && !HasId)
                    {
                        _hideItems.Add(property.Name);
                        continue;
                    }

                    var attributes = property.GetCustomAttributes(true);
                    var itemAttr = GetAttribute<FormItemAttribute>(attributes);

                    if (name == remarkName && ModelTagHelper.IsRemarkType)
                    {
                        var remarkTag = new TextboxTagHelper();
                        remarkTag.IsMultiline = true;
                        remarkTag.MaxLength = DomainConsts.MaxDescLength;

                        var textAreaTag = CreateItemTag(remarkName, WebConsts.DisplayName_Remark, remarkTag);

                        textAreaTag.Width = itemAttr == null || itemAttr.Width.IsNullOrWhiteSpace() ? "100%" : itemAttr.Width;
                        _textAreas.Add(textAreaTag);

                        continue;
                    }

                    index++;


                    var valueType = propertyType.GetValueType();
                    var isNullType = propertyType.IsNullableType();
                    var isEnum = valueType.IsEnum;
                    var isValueType = propertyType.IsValueType && !isNullType;
                    var isName = ModelTagHelper.IsNameType && name == nameof(IHasName.Name);
                    var isDisable = GetAttribute<DisableAttribute>(attributes) != null || !property.CanWrite;
                    var colNameAttr = GetAttribute<DisplayNameAttribute>(attributes);
                    var requiredAttr = GetAttribute<RequiredAttribute>(attributes);
                    var comboboxAttr = GetAttribute<ComboboxAttribute>(attributes);

                    var isCombo = comboboxAttr != null || propertyType == typeof(bool) || propertyType == typeof(bool?) || valueType.IsEnum;
                    var colName = colNameAttr == null ? name : colNameAttr.DisplayName;


                    if (name == colName)
                    {
                        if (isFileType)
                            colName = "文件";
                        else if (isEnum)
                            colName = valueType.GetDescription();
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

                                var comboAttr = GetAttribute<ComboboxAttribute>(attributes);


                                if (comboAttr != null)
                                {
                                    if (comboAttr.WhereField.IsNotNullOrWhiteSpace())
                                    {
                                        tag.WhereField = comboAttr.WhereField;
                                        tag.WhereValue = comboAttr.WhereValue;
                                        tag.WhereOper = comboAttr.WhereOper;
                                    }

                                    tag.IsEdit = !comboAttr.IsReadOnly;
                                }
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
                        var precisionAttr = GetAttribute<NumberFormatAttribute>(attributes);
                        itemTag = new NumberboxTagHelper
                        {
                            Precision = precisionAttr?.Precision
                        };
                    }
                    else if (valueType == typeof(DateTime))
                    {
                        var datetimeAttr = GetAttribute<DateFormatAttribute>(attributes);
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

                        var isEmail = GetAttribute<EmailAddressAttribute>(attributes) != null;
                        if (isEmail)
                        {
                            tag.ValidTypes.Add("email");
                        }

                        var isIdCard = GetAttribute<IdCardAttribute>(attributes) != null;
                        if (isIdCard)
                        {
                            tag.ValidTypes.Add("idCard");
                        }

                        var isPhone = GetAttribute<PhoneAttribute>(attributes) != null || GetAttribute<PhoneNumberAttribute>(attributes) != null;
                        if (isPhone)
                        {
                            tag.ValidTypes.Add("phone");
                        }

                        var isMobilePhone = GetAttribute<MobilePhoneAttribute>(attributes) != null;
                        if (isMobilePhone)
                        {
                            tag.ValidTypes.Add("mobile");
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

                    FormItemTagHelper formItem;

                    if (isTextArea)
                    {
                        formItem = CreateItemTag(name, colName, itemTag, index);
                        _textAreas.Add(formItem);
                    }
                    else
                    {
                        formItem = CreateItemTag(name, colName, itemTag, isId ? 0 : index);
                        _formItems.Add(formItem);
                    }


                    if (itemAttr != null)
                    {
                        if (itemAttr.After.IsNotNullOrWhiteSpace())
                            formItem.After = itemAttr.After;
                        if (itemAttr.Before.IsNotNullOrWhiteSpace())
                            formItem.Before = itemAttr.Before;
                        if (itemAttr.ColSpan.HasValue)
                            formItem.ColSpan = itemAttr.ColSpan;
                        if (itemAttr.Group.IsNotNullOrWhiteSpace())
                            formItem.Group = itemAttr.Group;
                        if (itemAttr.Width.IsNotNullOrWhiteSpace())
                            formItem.Width = itemAttr.Width;
                    }
                }
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
            if (ItemWidth.HasValue && !tag.ColSpan.HasValue)
            {
                tag.Width = $"{ItemWidth}px";
            }
            return tag;
        }
    }
}
