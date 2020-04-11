using System;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// DataRow转实体对象
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="dr">要转换的DataRow</param>
        /// <param name="action">转换后要执行的操作</param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow dr, Action<T> action = null)
        {
            var temp = typeof(T);
            var entity = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (var property in temp.GetProperties())
                {
                    if (property.Name == column.ColumnName)
                    {
                        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        var value = dr[column.ColumnName];
                        object safeValue = value.ToString(null).IsNullOrWhiteSpace() ? null : Convert.ChangeType(value, type);//类型强转，将table字段类型转为集合字段类型
                        property.SetValue(entity, safeValue, null);
                    }
                }
            }

            action?.Invoke(entity);
            return entity;
        }

        public static T GetValueOrDefault<T>(this DataRow row, string colName, T defaultValue = default) where T : struct
        {
            return row.Table.Columns.Contains(colName) ? row[colName].To<T>(defaultValue) : default;
        }
    }
}
