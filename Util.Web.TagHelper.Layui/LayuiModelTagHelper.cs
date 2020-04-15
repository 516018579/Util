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

        private Type _modelType;
        public Type ModelType
        {
            get => _modelType;
            set
            {
                _modelType = value;
                ModelTagHelper = new ModelTagHelper(_modelType);
            }
        }

        protected LayuiModelTagHelper()
        {
            ModelTagHelper = new ModelTagHelper();
        }
    }
}
