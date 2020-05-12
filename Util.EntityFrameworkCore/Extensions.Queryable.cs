using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Util.EntityFrameworkCore
{
    public static partial class EntityFrameworkCoreExtensions
    {
        public static IQueryable Set(this DbContext context, Type T)
        {

            // Get the generic type definition
            MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);

            // Build a method with the specific type argument you're interested in
            method = method.MakeGenericMethod(T);

            return method.Invoke(context, null) as IQueryable;
        }

        public static Task<TResult> MaxOrDefaultAsync<T, TResult>(this IQueryable<T> enumerable, Expression<Func<T, TResult>> selector)
        {
            return enumerable.Select(selector).DefaultIfEmpty().MaxAsync();
        }

        public static Task<TResult> SelectFirstAsync<T, TResult>(this IQueryable<T> queryable, Expression<Func<T, TResult>> selector)
        {
            return queryable.Select(selector).FirstOrDefaultAsync();
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> collection, IEnumerable<SqlFilter> filters)
        {
            if (filters != null && filters.Any())
            {
                var fiterList = filters.Where(x => !string.IsNullOrWhiteSpace(x.Field)).ToList();
                if (fiterList.Any())
                {
                    var i = 0;
                    StringBuilder where = new StringBuilder();
                    var valueList = new object[fiterList.Count];
                    foreach (var filter in fiterList)
                    {
                        if (i > 0)
                        {
                            if (filter.IsAnd)
                            {
                                where.Append(" and ");
                            }
                            else
                            {
                                where.Append(" or ");
                            }
                        }
                        where.Append(filter.Field + filter.Oper + "@" + i + " ");

                        dynamic value;

                        switch (filter.ValueType)
                        {
                            case ValueType.整数:
                                value = Convert.ToInt32(filter.Value);
                                break;
                            case ValueType.时间:
                                value = Convert.ToDateTime(filter.Value);
                                break;
                            case ValueType.布尔:
                                value = Convert.ToBoolean(filter.Value);
                                break;
                            case ValueType.小数:
                                value = Convert.ToDecimal(filter.Value);
                                break;
                            case ValueType.长整数:
                                value = Convert.ToInt64(filter.Value);
                                break;
                            case ValueType.唯一标识符:
                                value = Guid.Parse(filter.Value.ToString());
                                break;
                            default:
                                value = filter.Value;
                                break;
                        }
                        valueList[i] = value;
                        i++;
                    }
                    string whereSql = where.ToString();
                    collection = collection.Where(whereSql, valueList.ToArray());
                }
            }
            return collection;
        }

        public static IQueryable Where(this IQueryable collection, IEnumerable<SqlFilter> filters)
        {
            if (filters != null && filters.Any())
            {
                var fiterList = filters.Where(x => !string.IsNullOrWhiteSpace(x.Field)).ToList();
                if (fiterList.Any())
                {
                    var i = 0;
                    StringBuilder where = new StringBuilder();
                    var valueList = new object[fiterList.Count];
                    foreach (var filter in fiterList)
                    {
                        if (i > 0)
                        {
                            if (filter.IsAnd)
                            {
                                where.Append(" and ");
                            }
                            else
                            {
                                where.Append(" or ");
                            }
                        }
                        where.Append(filter.Field + (filter.Oper ?? "=") + "@" + i + " ");

                        dynamic value;
                        switch (filter.ValueType)
                        {
                            case ValueType.整数:
                                value = Convert.ToInt32(filter.Value);
                                break;
                            case ValueType.时间:
                                value = Convert.ToDateTime(filter.Value);
                                break;
                            case ValueType.布尔:
                                value = Convert.ToBoolean(filter.Value);
                                break;
                            case ValueType.小数:
                                value = Convert.ToDecimal(filter.Value);
                                break;
                            case ValueType.长整数:
                                value = Convert.ToInt64(filter.Value);
                                break;
                            case ValueType.唯一标识符:
                                value = Guid.Parse(filter.Value.ToString());
                                break;
                            default:
                                value = filter.Value;
                                break;
                        }
                        valueList[i] = value;
                        i++;
                    }
                    string whereSql = where.ToString();
                    collection = collection.Where(whereSql, valueList.ToArray());
                }
            }
            return collection;
        }

    }
}
