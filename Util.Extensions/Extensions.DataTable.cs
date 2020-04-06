using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// DataTable转IEnumerable实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dt">要转换的DataTable</param>
        /// <param name="action">转换后要执行的操作</param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt, Action<T> action = null)
        {
            foreach (DataRow row in dt.Rows)
            {
                T data;
                try
                {
                    data = row.ToEntity(action);
                }
                catch (Exception e)
                {
                    throw;
                }

                yield return data;
            }
        }
    }
}
