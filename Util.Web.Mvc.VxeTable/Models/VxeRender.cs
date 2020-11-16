using System.Collections.Generic;
using Util.Application.Dto;

namespace Util.Web.Mvc.VxeTable
{
    public class VxeRender
    {
        public string Name { get; set; } = VxeConsts.RenderName_Input;
        public List<LabelValue> Options { get; set; } 
        public Dictionary<string, object> Props { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Events { get; set; } = new Dictionary<string, object>();
        public object DefaultValue { get; set; }

        public string Html { get; set; }
    }
}
