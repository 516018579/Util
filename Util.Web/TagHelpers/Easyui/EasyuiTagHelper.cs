using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Easyui
{
    public abstract class EasyuiTagHelper : BaseTagHelper
    {
        protected abstract string ClassName { get; }
        protected abstract string TagName { get; }
        protected virtual bool HasChild { get; } = true;

        public HashSet<string> Class = new HashSet<string>();

        protected readonly Dictionary<string, object> Options = new Dictionary<string, object>();
        protected string Option => Options.Select(x =>
        {
            var value = x.Value.ToString();
            if (!(x.Value is string))
            {
                value = value.ToCamelCase();
            }
            return $"{x.Key}:{value}";
        }).JoinAsString();


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            InitOption(context, output);

            output.TagName = TagName;
            output.AddClass(ClassName, HtmlEncoder.Default);

            foreach (var item in Class)
            {
                output.AddClass(item, HtmlEncoder.Default);
            }

            if (HasChild)
            {
                //添加默认内容
                var content = await output.GetChildContentAsync();
                output.Content.AppendHtml(content);
            }

            await base.ProcessAsync(context, output);
        }

        protected virtual void GetOption(TagHelperContext context, TagHelperOutput output)
        {
            var option = context.AllAttributes[WebConsts.Easyui.Option].Value?.ToString();
            if (option.IsNotNullOrWhiteSpace())
            {
                foreach (var x in option.Split(','))
                {
                    var items = x.Split(':');
                    Options.AddOrUpdate(items[0], items[1]);
                }
            }
        }

        protected virtual void AddOption(TagHelperContext context, TagHelperOutput output)
        {

        }


        protected virtual void InitOption(TagHelperContext context, TagHelperOutput output)
        {
            var hasOption = context.AllAttributes.ContainsName(WebConsts.Easyui.Option);

            if (hasOption)
            {
                GetOption(context, output);
            }

            AddOption(context, output);

            if (Options.Any())
            {
                var html = new HtmlString(Option.Replace("\"", "'"));
                if (hasOption)
                {
                    output.Attributes.SetAttribute(WebConsts.Easyui.Option, html);
                }
                else
                {
                    output.Attributes.Add(WebConsts.Easyui.Option, html);
                }
            }
        }
    }
}
