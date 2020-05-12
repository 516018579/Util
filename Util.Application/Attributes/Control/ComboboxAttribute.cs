using System;
using System.ComponentModel;
using System.Reflection;

namespace Util.Application.Attributes.Control
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComboboxAttribute : Attribute
    {
        public ComboboxAttribute()
        {
            Type = null;
            IsLoadData = false;
        }

        public ComboboxAttribute(Type type, bool isLoadData = true, string whereField = null, object whereValue = null, string whereOper = "=")
        {
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
            Type = type;
            DisplayName = attribute == null ? type.Name : attribute.DisplayName;
            IsLoadData = isLoadData;
            WhereField = whereField;
            WhereOper = whereOper;
            WhereValue = whereValue;
        }

        public Type Type { get; set; }
        public bool IsLoadData { get; set; }
        public string DisplayName { get; set; }

        /// <summary>
        /// 查询条件字段
        /// </summary>
        public string WhereField { get; set; }
        /// <summary>
        /// 查询条件逻辑符
        /// </summary>
        public string WhereOper { get; set; }
        /// <summary>
        /// 查询条件内容
        /// </summary>
        public object WhereValue { get; set; }
    }
}
