using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Util.Application.Attributes.Control;
using Util.Application.Attributes.Format;
using Util.Application.Dto;
using Util.Domain;
using Util.Domain.Attributes;
using Util.Domain.Entities;
using Util.Extensions;
using Util.Json;
using GuidConverter = Util.Web.Mvc.Converter.Json.GuidConverter;

namespace Util.Web.Mvc.VxeTable
{
    public static class VxeExtensions
    {
        private static ConcurrentDictionary<Type, string> _typeModelDictionary = new ConcurrentDictionary<Type, string>();
        private static ConcurrentDictionary<Type, List<VxeColumn>> _typeColumnDictionary = new ConcurrentDictionary<Type, List<VxeColumn>>();
        private static ConcurrentDictionary<Type, string> _typeRulesDictionary = new ConcurrentDictionary<Type, string>();
        private static ConcurrentDictionary<Type, string> _gridTypeRulesDictionary = new ConcurrentDictionary<Type, string>();
        private static ConcurrentDictionary<Type, List<VxeFormItem>> _typeItemDictionary = new ConcurrentDictionary<Type, List<VxeFormItem>>();
        private static ConcurrentDictionary<Type, Dictionary<string, ComboboxAttribute>> _typeSelectDictionary = new ConcurrentDictionary<Type, Dictionary<string, ComboboxAttribute>>();
        private static PropertyInfo[] _colPropertyInfos = typeof(VxeColumn).GetProperties();
        private static PropertyInfo[] _itemPropertyInfos = typeof(VxeFormItem).GetProperties();

        public static IHtmlContent GetColumns(this IHtmlHelper htmlHelper, Type type, object minWidth = null)
        {
            if (type == null)
            {
                return ReturnNull(htmlHelper);
            }

            var cols = GetColumns(type);

            if (minWidth != null)
            {
                foreach (var col in cols)
                {
                    col.MinWidth = minWidth;
                }
            }

            if (_typeSelectDictionary.ContainsKey(type))
            {
                var selects = _typeSelectDictionary[type];
                foreach (var select in selects)
                {
                    var item = cols.FirstOrDefault(x => x.Field.Equals(select.Key, StringComparison.OrdinalIgnoreCase));

                    if (item != null)
                    {
                        var comboAttr = select.Value;

                        InitRenderOptions(item.EditRender, comboAttr);
                    }
                }
            }

            return htmlHelper.Raw(cols.ToJsonString(true, isCamelCase: true, ignoreNull: true, converters: new GuidConverter()));
        }

        public static List<VxeColumn> GetColumns(Type type)
        {
            List<VxeColumn> cols = new List<VxeColumn>();
            if (!_typeColumnDictionary.ContainsKey(type))
            {

                foreach (var property in type.GetProperties())
                {
                    if (property.IsDefined(typeof(ColHideAttribute)))
                        continue;
                    if (property.IsDefined(typeof(ComboboxAttribute)) && !property.IsDefined(typeof(GridItemAttribute)))
                        continue;


                    if (property.PropertyType.IsGenericType &&
                        property.PropertyType.IsAssignableFromGenericTypeInterface(typeof(IEnumerable<>)))
                    {
                        var genericTypes = property.PropertyType.GetGenericArguments();
                        if (genericTypes.Length > 1 || !genericTypes[0].IsValueType())
                        {
                            continue;
                        }
                    }

                    var col = new VxeColumn();
                    col.Field = property.Name.ToCamelCase();
                    if (col.Field == "id")
                    {
                        col.Visible = false;
                    }

                    var propertyType = property.PropertyType;
                    var valueType = propertyType.GetValueType();
                    var isEnum = valueType.IsEnum;
                    var name = property.GetDisplayName();

                    if (isEnum)
                        name = valueType.GetDisplayName();
                    if (name == property.PropertyType.Name)
                    {
                        name = valueType.GetDescription();
                    }

                    if (isEnum)
                    {
                        col.Formatter = new JRaw($"['formatEnum', '{valueType.Name}']");
                    }
                    else
                    {
                        var comAttr = property.GetCustomAttribute<ComboboxAttribute>();
                        if (comAttr != null && comAttr.Type != null)
                        {
                            col.Formatter = new JRaw($"['formatTable', '{comAttr.Type.Name}']");
                        }
                    }


                    col.Title = name;

                    var itemAttr = property.GetCustomAttribute<GridItemAttribute>();
                    if (itemAttr != null)
                    {
                        if (itemAttr.Formatter.IsNotNullOrWhiteSpace())
                            col.Formatter = new JRaw(itemAttr.Formatter);

                        if (itemAttr.Width.IsNotNullOrWhiteSpace())
                            col.Width = itemAttr.Width;

                        if (itemAttr.IsEdit)
                        {
                            col.EditRender = new VxeGridItemEditRender();
                            var editRender = col.EditRender;

                            InitRender(type, editRender, property);
                        }
                    }


                    cols.Add(col);
                }

                _typeColumnDictionary.TryAdd(type, cols);
            }
            else
            {
                cols.AddRange(_typeColumnDictionary[type]);
            }


            return cols;
        }

        public static IHtmlContent GetItems(this IHtmlHelper htmlHelper, Type type, bool showId = false, int span = 12, bool isFolding = false, int? foldingStartIndex = null)
        {
            if (type == null)
            {
                return ReturnNull(htmlHelper);
            }
            var items = GetItemList(type, span, showId, isFolding, foldingStartIndex);

            return htmlHelper.Raw(items.ToJsonString(true, isCamelCase: true, ignoreNull: true, converters: new GuidConverter()));
        }

        public static List<VxeFormItem> GetItemList(Type type, int span = 12, bool showId = false, bool isFolding = false,
            int? foldingStartIndex = null)
        {
            List<VxeFormItem> items = new List<VxeFormItem>();
            var remarkName = nameof(IHasRemark.Remark);
            if (!_typeItemDictionary.ContainsKey(type))
            {
                foreach (var property in type.GetProperties())
                {
                    var item = new VxeFormItem();
                    item.DefaultSpan = (uint)span;
                    var itemRender = item.ItemRender;
                    item.Field = property.Name.ToCamelCase();
                    var name = property.Name;
                    var isId = name == nameof(WebConsts.Id);
                    var propertyType = property.PropertyType;
                    if (property.IsDefined(typeof(NotFormItemAttribute), false))
                        continue;
                    var isFileType = propertyType == typeof(IFormFile);
                    if (!propertyType.IsValueType() && !isFileType && propertyType != typeof(object))
                        continue;


                    var attributes = property.GetCustomAttributes(true);
                    var colNameAttr = GetAttribute<DisplayNameAttribute>(attributes);

                    if (isId)
                    {
                        if (showId)
                        {
                            item.Title = colNameAttr?.DisplayName ?? item.Field;
                            items.Insert(0, item);
                            if (property.IsDefined(typeof(HideFormItemAttribute), false))
                            {
                                item.VisibleMethod = GetFunction("return false");
                            }
                        }

                        continue;
                    }

                    if (name == remarkName)
                    {
                        item.ItemRender.Name = VxeConsts.RenderName_TextArea;
                        item.ItemRender.Props.Add("maxlength", DomainConsts.MaxDescLength);
                        item.ColSpan = 24 / (uint)span;
                    }

                    var valueType = propertyType.GetValueType();
                    var isNullType = propertyType.IsNullableType();
                    var isEnum = valueType.IsEnum;
                    var isValueType = propertyType.IsValueType && !isNullType;
                    var isName = name == nameof(IHasName.Name);
                    var isDisable = GetAttribute<DisableAttribute>(attributes) != null || !property.CanWrite;


                    if (isDisable)
                    {
                        item.Disabled = true;
                        item.ItemRender.Props.Add("disabled", true);
                    }

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
                                default:
                                    if (valueType.IsEnum)
                                        colName = valueType.GetDisplayName();
                                    break;
                            }
                        }
                    }

                    var comboboxAttr = GetAttribute<ComboboxAttribute>(attributes);
                    if (colNameAttr == null && comboboxAttr?.DisplayName != null)
                    {
                        colName = comboboxAttr.DisplayName;
                    }

                    var isTextArea = false;
                    int maxLength = DomainConsts.DefaultStringLength;

                    InitRender(type, itemRender, property);
                    //else if (isFileType)
                    //{
                    //    itemTag = new FileTagHelper();
                    //}

                    var itemAttr = GetAttribute<FormItemAttribute>(attributes);

                    if (itemAttr?.ColSpan > 0)
                    {
                        item.ColSpan = itemAttr.ColSpan.Value;
                    }

                    item.Title = colName;

                    items.Add(item);
                }

                _typeItemDictionary.TryAdd(type, items);
            }
            else
            {
                items.AddRange(_typeItemDictionary[type]);
            }

            if (_typeSelectDictionary.ContainsKey(type))
            {
                var selects = _typeSelectDictionary[type];
                foreach (var select in selects)
                {
                    var item = items.First(x => x.Field.Equals(@select.Key, StringComparison.OrdinalIgnoreCase));

                    var comboAttr = @select.Value;

                    InitRenderOptions(item.ItemRender, comboAttr);
                }
            }

            var index = 1;

            var fStartIndex = foldingStartIndex ?? 24 / span;

            foreach (var item in items)
            {
                item.DefaultSpan = (uint)span;
                if (item.Span > 24)
                {
                    item.DefaultSpan = 24 / item.DefaultSpan;
                }

                if (isFolding && index > fStartIndex)
                {
                    item.Folding = true;
                }

                index++;
            }

            return items;
        }

        private static void InitRenderOptions(VxeRender itemRender, ComboboxAttribute comboAttr)
        {
            itemRender.Options = WebConsts
                .GetComboboxDataFunc(comboAttr.Type.Name, comboAttr.WhereField, comboAttr.WhereOper,
                    comboAttr.WhereValue, comboAttr.DisplayField).ConfigureAwait(false).GetAwaiter().GetResult()
                .Select(x => new LabelValue(x.Key, x.Value)).ToList();
        }

        private static void InitRender(Type type, VxeRender itemRender, PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(true);
            var propertyType = property.PropertyType;
            var valueType = propertyType.GetValueType();
            var comboboxAttr = GetAttribute<ComboboxAttribute>(attributes);
            var isEnum = valueType.IsEnum;
            var isCombo = comboboxAttr != null || valueType.IsEnum;
            if (isCombo)
            {
                itemRender.Name = VxeConsts.RenderName_Iview_Select;
                itemRender.Props.Add("clearable", true);

                bool isFilter = true;
                if (isEnum)
                {
                    isFilter = false;
                    itemRender.Options = EnumUtil.GetEnumValueList(valueType).Select(x => new LabelValue(x.Key, x.Value))
                        .ToList();
                }
                else if (comboboxAttr != null)
                {
                    if (comboboxAttr.IsLoadData)
                    {
                        var comboAttr = GetAttribute<ComboboxAttribute>(attributes);

                        if (comboAttr != null && comboAttr.IsLoadData)
                        {
                            var selectDictionary = _typeSelectDictionary.GetOrDefault(type) ??
                                                   new Dictionary<string, ComboboxAttribute>();
                            selectDictionary.Add(property.Name, comboAttr);
                            if (!_typeSelectDictionary.ContainsKey(type))
                            {
                                _typeSelectDictionary.TryAdd(type, selectDictionary);
                            }
                        }
                    }
                }

                if (comboboxAttr?.IsReadOnly.HasValue == true)
                {
                    isFilter = comboboxAttr.IsReadOnly.Value;
                }

                if (isFilter)
                {
                    itemRender.Props.Add("filterable", true);
                }
            }
            else if (propertyType.IsNumberType())
            {
                var numberType = "number";
                if (!new[] { typeof(int), typeof(long) }.Contains(valueType))
                {
                    numberType = "float";
                    var precisionAttr = GetAttribute<NumberFormatAttribute>(attributes);
                    if (precisionAttr != null)
                    {
                        itemRender.Props.Add("digits", precisionAttr.Precision);
                    }
                }

                itemRender.Props.Add("type", numberType);

                var rangeAttr = GetAttribute<RangeAttribute>(attributes);
                if (rangeAttr != null)
                {
                    itemRender.Props.Add("min", rangeAttr.Minimum);
                    itemRender.Props.Add("max", rangeAttr.Maximum);
                }
                else
                {
                    itemRender.Props.Add("min", 0);
                }
            }
            else if (valueType == typeof(DateTime))
            {
                var datetimeAttr = GetAttribute<DateFormatAttribute>(attributes);
                var datePickerType = VxeConsts.RenderName_Iview_DatePicker;
                var dateType = "date";
                //itemRender.Props.Add("transfer", true);
                if (datetimeAttr != null)
                {
                    if (datetimeAttr.DateFormatString.StartsWith("HH"))
                    {
                        datePickerType = VxeConsts.RenderName_Iview_TimePicker;
                    }
                    else if (datetimeAttr.DateFormatString.Contains("HH"))
                    {
                        dateType = "datetime";
                    }

                    itemRender.Props.Add("format", datetimeAttr.DateFormatString);
                }

                if (datePickerType == VxeConsts.RenderName_Iview_DatePicker)
                {
                    itemRender.Props.Add("type", dateType);
                }

                itemRender.Name = datePickerType;
            }
            else if (propertyType.GetValueType() == typeof(bool))
            {
                itemRender.Name = VxeConsts.RenderName_Switch;
                itemRender.Props.Add("onLabel", "是");
                itemRender.Props.Add("offLabel", "否");
            }
        }

        public static IHtmlContent GetModel(this IHtmlHelper htmlHelper, Type type)
        {
            if (type == null)
            {
                return ReturnNull(htmlHelper, false);
            }
            if (!_typeModelDictionary.ContainsKey(type))
            {
                _typeModelDictionary.TryAdd(type, Activator.CreateInstance(type).ToJsonString(true, isCamelCase: true, converters: new GuidConverter()));
            }
            return htmlHelper.Raw(_typeModelDictionary[type]);
        }

        public static IHtmlContent GetRules(this IHtmlHelper htmlHelper, Type type, bool isGird = false)
        {
            if (type == null)
            {
                return ReturnNull(htmlHelper, false);
            }

            var rulesDictionary = isGird ? _gridTypeRulesDictionary : _typeRulesDictionary;

            if (!rulesDictionary.ContainsKey(type))
            {
                var rules = GetRule(type, isGird);

                rulesDictionary.TryAdd(type, rules.ToJsonString(true, isCamelCase: true, converters: new GuidConverter()));
            }

            return htmlHelper.Raw(rulesDictionary[type]);
        }

        public static IHtmlContent GetItemHtml(this IHtmlHelper htmlHelper, Type type, string modelName = null,
            Action<List<VxeFormItem>> action = null, int span = 12, string separator = "", bool showId = false,
            bool isLabel = false, bool isFolding = false)
        {
            if (type == null)
            {
                return ReturnNull(htmlHelper);
            }
            var items = GetItemList(type, span, showId, isFolding);

            foreach (var item in items)
            {
                if (isLabel)
                {
                    item.ItemRender = null;
                    item.Html = $"{{{{ {(modelName == null ? "" : modelName + ".")}{item.Field.ToCamelCase()} }}}}";
                }
                else
                {
                    item.ItemRender.Props.AddOrUpdate("v-model", (modelName == null ? null : modelName + ".") + item.Field.ToCamelCase());
                }
            }



            action?.Invoke(items);

            return htmlHelper.GetItemHtml(items);
        }

        public static IHtmlContent GetItemHtml(this IHtmlHelper htmlHelper, List<VxeFormItem> items)
        {
            var html = new StringBuilder();

            foreach (var item in items)
            {
                html.Append("<vxe-form-item ");

                html.Append(GetAttr(item.Attrs));

                void AppendAttr(string attrName)
                {
                    var value = item.GetPropertyValue(attrName);
                    if (value != null)
                    {
                        html.Append(GetAttr(attrName, value));
                    }
                }

                foreach (var propertyInfo in _itemPropertyInfos)
                {
                    if (!new[] { nameof(item.Attrs), nameof(item.ItemRender), nameof(item.Html) }.Contains(propertyInfo.Name))
                    {
                        AppendAttr(propertyInfo.Name);
                    }
                }

                html.Append(">");

                html.Append(string.IsNullOrWhiteSpace(item.Html) ? GetRender(item.ItemRender) : item.Html);

                html.Append("</vxe-form-item>");
            }

            return htmlHelper.Raw(html.ToString());
        }

        public static IHtmlContent GetColsHtml(this IHtmlHelper htmlHelper, Type type, bool showId = false, Action<List<VxeColumn>> action = null)
        {
            var cols = GetColumns(type);

            if (!showId)
            {
                cols = cols.Where(x => x.Field != "Id").ToList();
            }

            action?.Invoke(cols);
            return htmlHelper.GetColsHtml(cols);
        }


        public static IHtmlContent GetColsHtml(this IHtmlHelper htmlHelper, List<VxeColumn> columns)
        {
            var html = new StringBuilder();

            foreach (var column in columns)
            {
                html.Append("<vxe-table-column ");

                html.Append(GetAttr(column.Attrs));

                void AppendAttr(string attrName)
                {
                    var value = column.GetPropertyValue(attrName);
                    if (value != null)
                    {
                        html.Append(GetAttr(attrName, value));
                    }
                }

                foreach (var propertyInfo in _colPropertyInfos)
                {
                    if (propertyInfo.Name == nameof(VxeColumn.Attrs))
                    {
                        continue;
                    }
                    AppendAttr(propertyInfo.Name);
                }

                html.Append("></vxe-table-column>");
            }

            return htmlHelper.Raw(html.ToString());
        }

        static string GetAttr(string attrName, object value)
        {
            string attr = "";
            if (value != null)
            {
                var valueType = value.GetType().GetValueType();

                switch (attrName)
                {
                    case nameof(VxeColumn.EditRender):
                    case nameof(VxeFormItem.ItemRender):
                        value = value.ToJsonString(true, isCamelCase: true, ignoreNull: true, converters: new GuidConverter());
                        break;
                }

                if (!attrName.Contains("-"))
                {
                    var names = Regex.Split(attrName, @"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace).Select(x => x.ToLower());

                    attrName = names.JoinAsString('-');
                }

                if (valueType == typeof(bool))
                {
                    value = value.ToString().ToCamelCase();
                }

                if (valueType != typeof(string))
                {
                    attrName = ":" + attrName;
                }

                attr = $" {attrName}=\"{value}\" ";
            }

            return attr;
        }

        static string GetAttr(Dictionary<string, object> attrs)
        {
            var html = new StringBuilder();
            foreach (var attr in attrs)
            {
                html.Append(" " + attr.Key);
                var value = attr.Value;
                if (value != null)
                {
                    var valueType = value.GetType().GetValueType();

                    if (valueType == typeof(bool))
                    {
                        value = value.ToString().ToCamelCase();
                    }

                    html.Append($"=\"{value}\" ");
                }

                html.Append(" ");
            }

            return html.ToString();
        }

        static string GetRender(VxeRender render)
        {
            var html = new StringBuilder();

            var tagName = "";

            switch (render.Name)
            {
                case VxeConsts.RenderName_Input:
                    tagName = VxeConsts.TagName_Input;
                    break;
                case VxeConsts.RenderName_TextArea:
                    tagName = VxeConsts.TagName_TextArea;
                    break;
                case VxeConsts.RenderName_Switch:
                    tagName = VxeConsts.TagName_Switch;
                    break;
                case VxeConsts.RenderName_Iview_Select:
                    tagName = VxeConsts.TagName_Iview_Select;
                    break;
                case VxeConsts.RenderName_Iview_TimePicker:
                    tagName = VxeConsts.TagName_Iview_TimePicker;
                    break;
                case VxeConsts.RenderName_Iview_DatePicker:
                    tagName = VxeConsts.TagName_Iview_DatePicker;
                    break;
            }

            html.Append($"<{tagName} ");

            foreach (var @event in render.Events)
            {
                html.Append($" @{@event.Key}=\"{@event.Value}\" ");
            }

            foreach (var prop in render.Props)
            {
                html.Append(GetAttr(prop.Key, prop.Value));
            }
            html.Append(" >");

            if (render.Options != null)
            {
                foreach (var x in render.Options)
                {
                    var pre = "";
                    var valueType = x.Value.GetType();
                    var isbool = valueType == typeof(bool);
                    if (isbool || valueType.IsNumberType())
                    {
                        pre = ":";
                        if (isbool)
                        {
                            x.Value = x.Value.ToString().ToCamelCase();
                        }
                    }

                    html.Append($"<i-option {pre}value=\"{x.Value}\" key=\"{x.Value}\">{x.Label}</i-option>");
                }
            }

            html.Append(render.Html);

            html.Append($"</{tagName}>");

            return html.ToString();
        }


        private static Dictionary<string, List<Dictionary<string, object>>> GetRule(Type type, bool isGird = false)
        {
            var rules = new Dictionary<string, List<Dictionary<string, object>>>();
            foreach (var property in type.GetProperties())
            {
                if (property.IsDefined(typeof(NotFormItemAttribute)))
                    continue;
                if (property.IsDefined(typeof(HideFormItemAttribute)))
                    continue;
                if (!property.PropertyType.IsValueType())
                    continue;
                if (isGird && property.GetCustomAttribute<GridItemAttribute>()?.IsEdit != true)
                    continue;


                var rule = new List<Dictionary<string, object>>();
                rules.Add(property.Name, rule);

                void AddRule(string key, object value, string message = null)
                {
                    var r = new Dictionary<string, object> { { key, value } };
                    if (message != null)
                    {
                        r.Add("message", message);
                    }
                    rule.Add(r);
                }

                var isString = property.PropertyType == typeof(string);

                if (isString)
                {
                    if (property.IsDefined(typeof(RequiredAttribute)) || property.Name == "Id")
                    {
                        AddRule("required", true, "请填写内容!");
                    }
                }
                else if (!property.PropertyType.IsNullableType())
                {
                    AddRule("required", true, "请填写内容!");
                }

                var ruleType = "";

                if (isString)
                {
                    int? maxLength = 50;
                    var minLength = 0;
                    var lengthAttr = property.GetCustomAttribute<MaxLengthAttribute>();
                    if (lengthAttr != null)
                    {
                        maxLength = lengthAttr.Length;
                    }
                    else
                    {
                        var stringLengthAttr = property.GetCustomAttribute<StringLengthAttribute>();
                        if (stringLengthAttr != null)
                        {
                            maxLength = stringLengthAttr.MaximumLength;
                            minLength = stringLengthAttr.MinimumLength;
                        }
                    }

                    if (maxLength > 0)
                    {
                        AddRule("max", maxLength);
                    }

                    if (minLength > 0)
                    {
                        AddRule("min", minLength);
                    }

                    var isEmail = property.GetCustomAttribute<EmailAddressAttribute>() != null;
                    if (isEmail)
                    {
                        AddRule("validator", new JRaw("validates.validateMail"));
                    }

                    var isIdCard = property.GetCustomAttribute<IdCardAttribute>() != null;
                    if (isIdCard)
                    {
                        AddRule("validator", new JRaw("validates.validateIdCard"));
                    }

                    var isPhone = property.GetCustomAttribute<PhoneAttribute>() != null || property.GetCustomAttribute<PhoneNumberAttribute>() != null;
                    if (isPhone)
                    {
                        AddRule("validator", new JRaw("validates.validatePhone"));
                    }

                    var isMobilePhone = property.GetCustomAttribute<MobilePhoneAttribute>() != null;
                    if (isMobilePhone)
                    {
                        AddRule("validator", new JRaw("validates.validateMobile"));
                    }
                }
                else if (property.PropertyType.IsNumberType())
                {
                    ruleType = "number";
                    object min = null;
                    object max = null;

                    var range = property.GetCustomAttribute<RangeAttribute>();
                    if (range != null)
                    {
                        min = range.Minimum;
                        max = range.Maximum;
                    }

                    if (min != null)
                    {
                        AddRule("min", min);
                    }

                    if (max != null)
                    {
                        AddRule("max", max);
                    }
                }

                if (ruleType.IsNotNullOrWhiteSpace())
                {
                    AddRule("type", ruleType);
                }
            }

            return rules;
        }

        private static JRaw GetFunction(string funcContent, string param = null)
        {
            return new JRaw($"function({param}){{ {funcContent} }}");
        }

        private static T GetAttribute<T>(IEnumerable<object> attributes) where T : Attribute
        {
            return attributes.OfType<T>().FirstOrDefault(x => x is T);
        }

        private static IHtmlContent ReturnNull(IHtmlHelper htmlHelper, bool isArray = true)
        {
            return htmlHelper.Raw(isArray ? "[]" : "{}");
        }
    }
}
