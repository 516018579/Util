using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Util.Domain
{
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public static class EnumUtil
    {
        public static string IsAble_Disable = "禁用";
        public static string IsAble_Enable = "启用";

        public static string Bool_True = "是";
        public static string Bool_False = "否";

        private static ConcurrentDictionary<Enum, string> _enumDictionary = new ConcurrentDictionary<Enum, string>();//缓存枚举描述
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<int, string>> _concurrentDictionary = new ConcurrentDictionary<Type, IReadOnlyDictionary<int, string>>();//缓存枚举描述

        public static Dictionary<string, Type> AllEnumType { get; set; }

        public static Dictionary<bool, string> IsAbleDictionary = new Dictionary<bool, string> { { true, IsAble_Enable }, { false, IsAble_Disable } };
        public static Dictionary<bool, string> BoolDictionary = new Dictionary<bool, string> { { true, Bool_True }, { false, Bool_False } };

        /// <summary>
        ///     根据枚举描述获取枚举值
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="desc">枚举描述</param>
        /// <returns></returns>
        public static int? GetEnumValue<T>(string desc)
            where T : Enum
        {
            return GetEnumValue(desc, typeof(T));
        }

        public static int? GetEnumValue(string desc, Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new NotImplementedException("当前类型不是枚举");
            }
            //遍历枚举值
            foreach (var enumValue in Enum.GetValues(enumType))
            {
                var name = enumValue.ToString();
                if (name == desc)
                    return Convert.ToInt32(enumValue);
                object[] objAttrs =
                    enumValue.GetType()
                        .GetField(name)
                        .GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objAttrs.Length > 0)
                {
                    if (objAttrs[0] is DescriptionAttribute descAttr && desc == descAttr.Description)
                    {
                        return Convert.ToInt32(enumValue);
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     根据枚举名称获取枚举值和描述
        /// </summary>
        /// <param name="enumName">枚举名称</param>
        /// <returns></returns>
        public static IReadOnlyDictionary<int, string> GetEnumValueList(string enumName)
        {
            try
            {
                IReadOnlyDictionary<int, string> list = new Dictionary<int, string>();
                if (!string.IsNullOrEmpty(enumName))
                {
                    var enumType = AllEnumType.ContainsKey(enumName) ? AllEnumType[enumName] : null;
                    if (enumType == null)
                    {
                        throw new Exception("没有找到枚举:" + enumName);
                    }

                    list = GetEnumValueList(enumType);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     根据枚举类型获取获取枚举值和描述
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static IReadOnlyDictionary<int, string> GetEnumValueList<T>()
            where T : Enum
        {
            var enumType = typeof(T);
            return GetEnumValueList(enumType);
        }

        public static IReadOnlyDictionary<int, string> GetEnumValueList(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new NotImplementedException("当前类型不是枚举");
            }
            var list = _concurrentDictionary.GetOrAdd(enumType,
                _ => (from object enumValue in Enum.GetValues(enumType)
                      let text = enumValue.GetType()
                          .GetField(enumValue.ToString())
                          .GetDescription()
                      let value = Convert.ToInt32(enumValue)
                      select new { Value = value, Text = text }).ToDictionary(x => x.Value, x => x.Text));
            return list;
        }

        private static string GetDescription(this FieldInfo field)
        {
            var att = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), false);
            return att == null ? field.Name : ((DescriptionAttribute)att).Description;
        }

        public static string GetDescription(this Enum value)
        {
            return _enumDictionary.GetOrAdd(value, (key) =>
            {
                var type = key.GetType();
                var field = type.GetField(key.ToString());
                //如果field为null则应该是组合位域值，
                return field == null ? key.GetDescriptions() : GetDescription(field);
            });
        }

        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 获取位域枚举的描述，多个按分隔符组合
        /// </summary>
        public static string GetDescriptions(this Enum @this, string separator = ",")
        {
            var names = @this.ToString().Split(',');
            string[] res = new string[names.Length];
            var type = @this.GetType();
            for (int i = 0; i < names.Length; i++)
            {
                var field = type.GetField(names[i].Trim());
                if (field == null) continue;
                res[i] = GetDescription(field);
            }
            return string.Join(separator, res);
        }

        /// <summary>
        /// 判断值是否是枚举的值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue<T>(object value)
            where T : Enum
        {
            try
            {
                var enumType = typeof(T);
                if (value == null || value.ToString() == "")
                {
                    return false;
                }
                var enumValue = Enum.Parse(enumType, value.ToString());
                string str = enumValue.ToString();
                FieldInfo field = enumType.GetField(str);
                return field != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
