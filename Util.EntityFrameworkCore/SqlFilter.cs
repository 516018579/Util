using System;
using System.Collections.Generic;
using System.Text;

namespace Util.EntityFrameworkCore
{
    public class SqlFilter
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 运算符
        /// </summary>
        public string Oper { get; set; } = "=";

        public bool IsAnd { get; set; } = true;
        /// <summary>
        /// 值的类型(0:string, 1:int, 2:datetime, 3:bool, 4:decimal, 5:long)
        /// </summary>
        public int ValueType { get; set; }
    }
}
