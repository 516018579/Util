using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json.Linq;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers
{
    public abstract class TagHelperBase : TagHelper
    {
        protected virtual string ClassName { get; }
        protected abstract string TagName { get; }
        protected virtual bool HasChild { get; } = true;

        public HashSet<string> Class = new HashSet<string>();

        /// <summary>
        /// 配置属性名
        /// </summary>
        protected virtual string OptionName { get; }

        /// <summary>
        /// 配置项的值是否采用CamelCase命名
        /// </summary>
        public virtual bool ValueToCamelCase => false;

        protected Dictionary<string, object> Options { get; set; } = new Dictionary<string, object>();

        protected virtual string Option => Options.ToJsonString(isCamelCase: true);

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            InitOption(context, output);

            output.TagName = TagName;

            if (ClassName.IsNotNullOrWhiteSpace())
            {
                output.AddClass(ClassName, HtmlEncoder.Default);
            }
           

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
            if (OptionName != null)
            {
                var option = context.AllAttributes[OptionName].Value?.ToString();
                if (option.IsNotNullOrWhiteSpace())
                {
                    Options = JObject.Parse(option).ToDictionary();
                }
            }
        }

        protected virtual void AddOption(TagHelperContext context, TagHelperOutput output)
        {

        }


        protected virtual void InitOption(TagHelperContext context, TagHelperOutput output)
        {
            var hasOption = context.AllAttributes.ContainsName(OptionName);

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
                    output.Attributes.SetAttribute(OptionName, html);
                }
                else
                {
                    output.Attributes.Add(OptionName, html);
                }
            }
        }

        protected TagHelperOutput CreateTagHelperOutput()
        {
            return new TagHelperOutput(
                  tagName: "",
                  attributes: new TagHelperAttributeList(),
                  getChildContentAsync: (useCachedResult, encoder) =>
                      Task.Factory.StartNew<TagHelperContent>(
                          () => new DefaultTagHelperContent()
                      )
              );
        }

        protected async Task<string> RenderInnerTagHelper(TagHelper innerTagHelper, TagHelperContext context, TagHelperOutput output, bool hasTagName = true)
        {
            // Process the InnerTagHelper instance 
            await innerTagHelper.ProcessAsync(context, output);

            return !hasTagName ? output.Content.GetContent() : $"<{output.TagName}{RenderHtmlAttributes(output)}>{output.Content.GetContent()}</{output.TagName}>";
        }

        protected string RenderHtmlAttributes(TagHelperOutput output)
        {
            var builder = new StringBuilder();
            if (output.Attributes.Any())
                builder.Append(" ");
            foreach (var attribute in output.Attributes)
            {
                builder.Append($"{attribute.Name}=\"{attribute.Value}\"");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 去除前后换行符
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string HtmlTrim(string html)
        {
            return html.Trim().TrimAll("\n", "\r");
        }

        /// <summary>
        /// 在字符串开头和结尾添加符号
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="symbol">符号</param>
        /// <returns></returns>
        public string GetString(string value, string symbol = "'")
        {
            return $"{symbol}{value}{symbol}";
        }

        public static IHtmlDocument ParseHtml(string html, bool hasBody = false)
        {
            return new HtmlParser().ParseDocument(hasBody ? html : $"<html> <head> </head> <body> <table> <thead>{html} </thead> </table> </body> </html>");
        }
    }
}
