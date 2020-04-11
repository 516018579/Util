using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Util.Domain.Entities;

namespace Util.Web.TagHelpers.Layui
{
    public abstract class LayuiModelTagHelper : LayuiTagHelper
    {
        protected ModelTagHelper ModelTagHelper;

        public Type ModelType
        {
            set => ModelTagHelper = new ModelTagHelper(value);
        }
    }
}
