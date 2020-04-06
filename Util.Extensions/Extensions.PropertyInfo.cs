using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        ///   确定是否将任何自定义特性应用于类型的成员。
        ///    参数指定要搜索的成员和自定义特性的类型。
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="attributeType">特性类型</param>
        /// <returns></returns>
        public static bool IsDefined(this PropertyInfo property, Type attributeType)
        {
            return Attribute.IsDefined(property, attributeType);
        }

        public static string GetDisplayName(this PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<DisplayNameAttribute>();
            return attribute == null ? property.Name : attribute.DisplayName;
        }
    }
}
