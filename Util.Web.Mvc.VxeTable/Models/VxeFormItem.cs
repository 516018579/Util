using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Util.Web.Mvc.VxeTable.Enums;

namespace Util.Web.Mvc.VxeTable
{
    public class VxeFormItem
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public uint Span => DefaultSpan * ColSpan;
        [JsonIgnore]
        public uint DefaultSpan { get; set; } = 12;
        [JsonIgnore]
        public uint ColSpan { get; set; } = 1;
        public bool? Folding { get; set; }
        public object Width { get; set; }
        public object TitleWidth { get; set; }
        public bool Disabled { get; set; }
        public string Html { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Align? Align { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Align? TitleAlign { get; set; }
        public JRaw VisibleMethod { get; set; }
        public VxeFormItemRender ItemRender { get; set; } = new VxeFormItemRender();

        public Dictionary<string, object> Attrs { get; set; } = new Dictionary<string, object>();
    }
}
