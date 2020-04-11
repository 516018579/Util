using System;

namespace Util.Application.Attributes.Format
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NumberFormatAttribute : Attribute
    {
        public NumberFormatAttribute()
        {
        }

        public NumberFormatAttribute(int precision)
        {
            Precision = precision;
        }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int Precision { get; set; } = 2;

        /// <summary>
        /// 结尾符号
        /// </summary>
        public string EndSymbol { get; set; }

        /// <summary>
        /// 计算方式
        /// </summary>
        public CalculateType CalculateType { get; set; }
    }

    public enum CalculateType
    {
        四舍五入,
        进一法,
        退一法
    }
}
