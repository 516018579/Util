using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Util.EntityFrameworkCore.Function
{
    public static class StringFunctions
    {
        /// <summary>
        /// left大于right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsGreaterThan(this string left, string right) => string.CompareOrdinal(left, right) > 0;
        /// <summary>
        ///  left大于等于right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsGreaterThanOrEqual(this string left, string right) => string.CompareOrdinal(left, right) >= 0;
        /// <summary>
        /// left小于right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsLessThan(this string left, string right) => string.CompareOrdinal(left, right) < 0;
        /// <summary>
        /// left小于等于right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsLessThanOrEqual(this string left, string right) => string.CompareOrdinal(left, right) <= 0;
        public static ModelBuilder RegisterStringFunctions(this ModelBuilder modelBuilder) => modelBuilder
            .RegisterFunction(nameof(IsGreaterThan), ExpressionType.GreaterThan)
            .RegisterFunction(nameof(IsGreaterThanOrEqual), ExpressionType.GreaterThanOrEqual)
            .RegisterFunction(nameof(IsLessThan), ExpressionType.LessThan)
            .RegisterFunction(nameof(IsLessThanOrEqual), ExpressionType.LessThanOrEqual);
        static ModelBuilder RegisterFunction(this ModelBuilder modelBuilder, string name, ExpressionType type)
        {
            var method = typeof(StringFunctions).GetMethod(name, new[] { typeof(string), typeof(string) });

            modelBuilder.HasDbFunction(method).HasTranslation(parameters =>
            {
                var left = parameters.ElementAt(0);
                var right = parameters.ElementAt(1);
                // EF Core 2.x
                //return Expression.MakeBinary(type, left, right, false, method);

                // EF Core 3.x
                if (right is SqlParameterExpression rightParam)
                    right = rightParam.ApplyTypeMapping(left.TypeMapping);
                else if (left is SqlParameterExpression leftParam)
                    left = leftParam.ApplyTypeMapping(right.TypeMapping);
                return new SqlBinaryExpression(type, left, right, typeof(bool), null);
            });
            return modelBuilder;
        }
    }
}
