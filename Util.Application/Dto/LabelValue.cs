using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Application.Dto
{
    public class LabelValue
    {
        public LabelValue(object value = null, string label = null)
        {
            Value = value;
            Label = label;
        }

        public object Value { get; set; }
        public string Label { get; set; }
    }
}
