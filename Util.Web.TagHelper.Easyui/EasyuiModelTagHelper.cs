using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Util.Domain.Entities;
using Util.Web.TagHelpers.Easyui;

namespace Util.Web.TagHelpers.Easyui
{
    public abstract class EasyuiModelTagHelper : EasyuiTagHelper
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
        protected EasyuiModelTagHelper()
        {
            ModelTagHelper = new ModelTagHelper();
        }
    }
}
