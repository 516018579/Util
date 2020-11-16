using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Util.Application.Dto;
using Util.Web.Mvc.VxeTable.Enums;

namespace Util.Web.Mvc.VxeTable
{
    public class VxeGridItemEditRender : VxeRender
    {
        public Dictionary<string, object> Attrs { get; set; } = new Dictionary<string, object>();
    }
}
