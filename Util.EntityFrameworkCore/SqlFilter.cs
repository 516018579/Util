using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Util.EntityFrameworkCore
{
    public class SqlFilter
    {
        public SqlFilter(string field, object value, ValueType? valueType = null, string oper = "=", bool isAnd = true)
        {
            Field = field;
            Value = value;
            Oper = oper;
            IsAnd = isAnd;
            ValueType = valueType;
        }

        public SqlFilter()
        {
            Oper = "=";
            IsAnd = true;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        public string FilterValue
        {
            get => Value?.ToString();
            set => Value = value;
        }

        /// <summary>
        /// 运算符
        /// </summary>
        public string Oper { get; set; }

        public bool IsAnd { get; set; }
        /// <summary>
        /// 值的类型(0:string, 1:int, 2:datetime, 3:bool, 4:decimal, 5:long)
        /// </summary>
        public ValueType? ValueType { get; set; }
    }

    public enum ValueType
    {
        /// <summary>
        /// string
        /// </summary>
        字符串,
        /// <summary>
        /// int
        /// </summary>
        整数,
        /// <summary>
        /// datetime
        /// </summary>
        时间,
        /// <summary>
        /// bool
        /// </summary>
        布尔,
        /// <summary>
        /// datetime
        /// </summary>
        小数,
        /// <summary>
        /// long
        /// </summary>
        长整数,
        /// <summary>
        /// guid
        /// </summary>
        唯一标识符
    }
}
