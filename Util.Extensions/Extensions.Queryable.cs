using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        public static TResult MaxOrDefault<T, TResult>(this IQueryable<T> queryable, Expression<Func<T, TResult>> selector)
        {
            return queryable.Select(selector).DefaultIfEmpty().Max();
        }

        public static TResult MinOrDefault<T, TResult>(this IQueryable<T> queryable, Expression<Func<T, TResult>> selector)
        {
            return queryable.Select(selector).DefaultIfEmpty().Min();
        }

        public static TResult SelectFirst<T, TResult>(this IQueryable<T> queryable, Expression<Func<T, TResult>> selector)
        {
            return queryable.Select(selector).FirstOrDefault();
        }
    }
}
