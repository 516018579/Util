using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Util.Domain.Entities;
using Util.Web.TagHelper.Easyui;

namespace Util.Web.TagHelpers.Easyui
{
    public abstract class ModelTagHelper : EasyuiTagHelper
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> ModelTypes = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, bool> RemarkTypes = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Type, bool> NameTypes = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Type, bool> SortTypes = new ConcurrentDictionary<Type, bool>();

        public Type ModelType { get; set; }

        protected bool HasModel => ModelType != null;
        protected bool IsNameType => NameTypes.GetOrAdd(ModelType, IsAssignableFrom<IHasName>());
        protected bool IsRemarkType => RemarkTypes.GetOrAdd(ModelType, IsAssignableFrom<IHasRemark>());
        protected bool IsSortType => SortTypes.GetOrAdd(ModelType, IsAssignableFrom<IHasSort>());

        protected PropertyInfo[] PropertyInfos
        {
            get
            {
                if (ModelType == null)
                    throw new Exception("请设置ModelType属性!");
                return ModelTypes.GetOrAdd(ModelType, ModelType.GetProperties());
            }

        }

        protected bool IsAssignableFrom<T>()
        {
            return typeof(T).IsAssignableFrom(ModelType);
        }
    }
}
