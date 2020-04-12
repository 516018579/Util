using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Domain;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("easyui-xm-select")]
    public class ComboboxTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "easyui-xm-select";
        protected override string TagName => "div";
        public Type ModelType { get; set; }
        [HtmlAttributeName("data")]
        public string Data { get; set; }
        [HtmlAttributeName("default-value")]
        public dynamic DefaultValue { get; set; }
        [HtmlAttributeName("default-index")]
        public int? DefaultIndex { get; set; }
        /// <summary>
        /// 是否包可搜索
        /// </summary>
        public bool Filterable { get; set; } = true;
        /// <summary>
        /// 是否单选
        /// </summary>
        public bool IsSingle { get; set; } = false;
        /// <summary>
        /// 点击选项后是否关闭
        /// </summary>
        public bool ClickClose { get; set; }

        /// <summary>
        /// 显示内容绑定的字段
        /// </summary>
        public string NameField { get; set; } = "text";

        /// <summary>
        /// 值绑定的字段
        /// </summary>
        public string ValueField { get; set; } = "value";

        /// <summary>
        /// 下拉框最大高度
        /// </summary>
        public string MaxHeigth { get; set; } = "200px";

        /// <summary>
        /// 最大选择数量
        /// </summary>
        public uint MaxSelect { get; set; }

        /// <summary>
        /// 是否包含全选,清空,和反选选项
        /// </summary>
        public bool HasToolBar { get; set; }

        public ComboxSize? Size { get; set; }

        public ComboboxTagHelper()
        {
            HasToolBar = ClickClose = IsSingle;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (ModelType != null)
            {
                var json = new List<Dictionary<string, object>>();
                Dictionary<object, string> list;
                if (ModelType.IsEnum)
                {
                    list = EnumUtil.GetEnumValueList(ModelType).ToDictionary(x => x.Key as object, x => x.Value);
                }
                else
                {
                    list = (await TagHelperConfig.GetComboboxDataFunc(ModelType.Name)).ToDictionary(x => x.Key as object, x => x.Value);
                }

                int i = 0;
                foreach (var item in list)
                {
                    var data = new Dictionary<string, object> { { NameField, item.Value }, { ValueField, item.Key } };

                    if (DefaultValue != null && DefaultValue.GetType().IsEnum)
                        DefaultValue = (int)DefaultValue;

                    if (item.Value == DefaultValue || i == DefaultIndex)
                    {
                        data.Add("selected", true);
                    }

                    json.Add(data);
                    i++;
                }

                Data = json.ToJsonString();
            }




            await base.ProcessAsync(context, output);
        }

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.Add("height", MaxHeigth);
            Options.Add(LayuiConsts.Select_Filterable, Filterable);
            Options.Add(LayuiConsts.Select_Single, IsSingle);
            Options.AddIf(IsRequired.HasValue, "layVerify", IsRequired);
            Options.AddIf(MaxSelect > 0, "max", MaxSelect);
            Options.AddIf(Size.HasValue, "size", Size.ToString().ToLower());
            Options.AddIf(IsDisable.HasValue, "disabled", IsDisable);
            Options.AddIf(Data.IsNotNullOrWhiteSpace(), "data", Data);

            Options.Add("model", new Dictionary<string, object> { { "icon", "hidden" } });
            Options.Add("prop", new Dictionary<string, object> { { LayuiConsts.Select_NameField, NameField }, { LayuiConsts.Select_ValueField, ValueField } });

            if (HasToolBar)
            {
                Options.Add("toolbar", new Dictionary<string, object> { { "show", true }, { "list", new[] { "ALL", "CLEAR", "REVERSE" } } });
            }

            base.AddOption(context, output);
        }
    }

    public enum ComboxSize
    {
        Large,
        Medium,
        Small,
        Mini
    }
}
