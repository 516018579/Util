using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Util.Web.Mvc.VxeTable.Enums;

namespace Util.Web.Mvc.VxeTable
{
    public class VxeColumn
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ColumnType? Type { get; set; }
        public string Field { get; set; }
        public string Title { get; set; }
        public object Width { get; set; }
        public object MinWidth { get; set; }
        public bool? Resizable { get; set; }
        public bool? Visible { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Fixed? Fixed { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Align? Align { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Align? HeaderAlign { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Align? FooterAlign { get; set; }
        public bool RemoteSort { get; set; }
        public JRaw Formatter { get; set; }
        public VxeGridItemEditRender EditRender { get; set; }
        public Dictionary<string, object> Attrs { get; set; } = new Dictionary<string, object>();
    }
}
