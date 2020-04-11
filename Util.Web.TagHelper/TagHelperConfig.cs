using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Util.Web.TagHelpers
{
    public class TagHelperConfig
    {
        /// <summary>
        /// 获取下拉框数据方法
        /// </summary>
        public static Func<string, Task<Dictionary<string, string>>> GetComboboxDataFunc;
    }
}
