﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Util.Extensions;
using Util.Json;
using Util.Web.TagHelpers;

namespace Util.Web.TagHelpers.Easyui
{
    public abstract class EasyuiTagHelper : TagHelperBase
    {
        protected override string OptionName => EasyuiConsts.Option;
    }
}
