using System;
using System.Linq;
using System.Linq.Expressions;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> sourceExpression, bool isAnd, Expression<Func<T, bool>> andExpression)
        {
            if (isAnd)
            {
                sourceExpression = sourceExpression.And(andExpression);
            }
            return sourceExpression;
        }

        /// <summary>
        /// 拼接 OR 条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> condition)
        {
            var inv = Expression.Invoke(condition, exp.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.Or(exp.Body, inv), exp.Parameters);
        }

        /// <summary>
        /// 拼接And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> condition)
        {
            var inv = Expression.Invoke(condition, exp.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.And(exp.Body, inv), exp.Parameters);
        }
    }
}
