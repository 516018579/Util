using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Util.Web
{
    public class WebConfig
    {
        /// <summary>
        /// 获取下拉框数据方法
        /// </summary>
        public static Func<string, Task<Dictionary<string, string>>> GetComboboxDataFunc;
    }
}
