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
        /// <summary>将双精度浮点值按指定的小数位数(四舍五入)。</summary>
        /// <param name="value">要舍入的双精度浮点数。</param>
        /// <param name="digits">返回值中的小数数字。</param>
        /// <returns>最接近 <paramref name="value" /> 的 <paramref name="digits" /> 位小数的数字。</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="digits" /> 为小于 0 或大于 15。(默认为2)</exception>
        public static double ToRund(this double value, int digits = 2)
        {
            return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 除法运算(当分子为0时返回0)
        /// </summary>
        /// <param name="denominator">分母</param>
        /// <param name="numerator">分子</param>
        /// <param name="defaultValue">当分子为0是的返回默认值</param>
        /// <param name="digits">保留位数</param>
        /// <returns></returns>
        public static double Division(this double denominator, double numerator, double defaultValue = 0, int digits = -1)
        {
            var value = defaultValue;
            if (numerator > 0 || numerator < 0)
            {
                value = denominator / numerator;
                if (digits >= 0)
                {
                    value = value.ToRund(digits);
                }
            }

            return value;
        }
    }
}
