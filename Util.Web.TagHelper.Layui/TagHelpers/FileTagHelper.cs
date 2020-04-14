using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;

namespace Util.Web.TagHelpers.Layui
{
    [HtmlTargetElement("layui-filebox")]
    public class FileTagHelper : TextboxTagHelper
    {
        protected override string ClassName => "layui-filebox layui-btn";
        protected override string TagName => "button";

        public string ButtonText { get; set; } = "文件上传";

        public FIleType FIleType { get; set; } = FIleType.File;

        public bool IsAtuoUpload { get; set; } = false;
        /// <summary>
        /// 设定文件域的字段名
        /// </summary>
        public string Field { get; set; } = "file";

        public string Url { get; set; }

        /// <summary>
        /// 规定打开文件选择框时，筛选出的文件类型，值为用逗号隔开的 MIME 类型列表。如： 
        /// acceptMime: 'image/*'（只显示图片文件） 
        /// acceptMime: 'image/jpg, image/png'（只显示 jpg 和 png 文件） 
        /// </summary>
        public string AcceptMime { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.Append(ButtonText);

            return base.ProcessAsync(context, output);
        }

        protected override void AddOption(TagHelperContext context, TagHelperOutput output)
        {
            Options.Add("accept", FIleType.ToString().ToLower());
            Options.Add("auto", IsAtuoUpload);
            Options.AddIf(AcceptMime.IsNotNullOrWhiteSpace(), "acceptMime", AcceptMime);
            Options.AddIf(Url.IsNotNullOrWhiteSpace(), "url", Url);
            Options.AddIf(Field.IsNotNullOrWhiteSpace(), "field", Field);

            base.AddOption(context, output);
        }
    }

    public enum FIleType
    {
        Images,
        File,
        Video,
        Audio
    }
}
