using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// 获取类型的Description,  没有返回类名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(this Type type)
        {
            var attribute = type.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? type.Name : attribute.Description;
        }

        /// <summary>
        /// 获取类型的DisplayName,  没有返回类名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Type type)
        {
            type = type.GetValueType();
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
            return attribute == null ? type.Name : attribute.DisplayName;
        }


        /// <summary>
        /// 获取值类型(只有Nullable类型有效)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetValueType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// 是否数字类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumberType(this Type type)
        {
            return Type.GetTypeCode(type.GetValueType()).IsNumberType();
        }

        public static bool IsNumberType(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 是否是值类型, 包含string类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsValueType(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        public static bool IsAssignableFrom<T>(this Type type)
        {
            var baseType = typeof(T);

            if (baseType.IsInterface && baseType.IsGenericType)
            {
                return type.IsAssignableFromGenericTypeInterface(baseType);
            }
            else
            {
                return baseType.IsAssignableFrom(type);
            }
        }

        public static bool IsAssignableFromGenericTypeInterface(this Type type, Type baseType)
        {
            return Array.Exists(type.GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == baseType);
        }

        public static string GetPath(this Type type)
        {
            return type.Assembly.GetPath();
        }

        public static bool IsNullableType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == (object)typeof(Nullable<>);
        }
    }
}
