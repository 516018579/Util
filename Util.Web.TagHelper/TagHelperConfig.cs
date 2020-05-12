using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Util.Web.TagHelpers
{
    public class TagHelperConfig
    {
        /// <summary>
        /// 获取下拉框数据方法
        /// 属性名, 条件字段, 条件判断符, 条件值
        /// </summary>
        public static Func<string, string, string, dynamic, Task<Dictionary<string, string>>> GetComboboxDataFunc { get; set; }
    }
}
