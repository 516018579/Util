using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;

namespace Util.Web.TagHelpers
{
    public class BaseTagHelper : TagHelper
    {
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

        public string HmlTrim(string html)
        {
            return html.Trim().TrimAll("\n", "\r");
        }

        public static IHtmlDocument ParseHtml(string html, bool hasBody = false)
        {
            return new HtmlParser().ParseDocument(hasBody ? html : $"<html> <head> </head> <body> <table> <thead>{html} </thead> </table> </body> </html>");
        }
    }
}
