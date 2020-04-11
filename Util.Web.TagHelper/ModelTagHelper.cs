using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Util.Domain.Entities;

namespace Util.Web.TagHelpers
{
    public class ModelTagHelper
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> ModelTypes = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, bool> RemarkTypes = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Type, bool> NameTypes = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Type, bool> SortTypes = new ConcurrentDictionary<Type, bool>();

        public readonly Type ModelType;

        public ModelTagHelper(Type modelType = null)
        {
            ModelType = modelType;
        }

        public bool HasModel => ModelType != null;
        public bool IsNameType => NameTypes.GetOrAdd(ModelType, IsAssignableFrom<IHasName>());
        public bool IsRemarkType => RemarkTypes.GetOrAdd(ModelType, IsAssignableFrom<IHasRemark>());
        public bool IsSortType => SortTypes.GetOrAdd(ModelType, IsAssignableFrom<IHasSort>());

        public PropertyInfo[] PropertyInfos
        {
            get
            {
                if (ModelType == null)
                    throw new Exception("请设置ModelType属性!");
                return ModelTypes.GetOrAdd(ModelType, ModelType.GetProperties());
            }

        }

        private bool IsAssignableFrom<T>()
        {
            return typeof(T).IsAssignableFrom(ModelType);
        }
    }
}
