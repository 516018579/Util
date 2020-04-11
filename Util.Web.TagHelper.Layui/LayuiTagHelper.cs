using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;

namespace Util.Web.TagHelpers.Layui
{
    public abstract class LayuiTagHelper : TagHelperBase
    {
        protected override string OptionName =>LayuiConsts.Option;
    }
}
