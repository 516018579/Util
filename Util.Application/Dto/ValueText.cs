﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Application.Dto
{
    public class ValueText
    {
        public ValueText(object value = null, string text = null)
        {
            Value = value;
            Text = text;
        }

        public object Value { get; set; }
        public string Text { get; set; }
    }
}
