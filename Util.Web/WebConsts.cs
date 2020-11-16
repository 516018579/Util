using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Util.Web
{
    public class WebConsts
    {
        /// <summary>
        /// 获取下拉框数据方法
        /// 属性名, 条件字段, 条件判断符, 条件值
        /// </summary>
        public static Func<string, string, string, dynamic, string, Task<Dictionary<object, string>>> GetComboboxDataFunc { get; set; }

        public static string WebRootPath { get; set; }

        public const string Id = "Id";
        public const string IsActive = "IsActive";

        public const string DisplayName_Name = "名称";
        public const string DisplayName_Remark = "备注";

        public const string Order_Asc = "asc";
        public const string Order_Desc = "desc";

        public const string IsAble_Disable = "禁用";
        public const string IsAble_Enable = "启用";
        public const string IsAble_Name = "是否启用";

        public const string Sort_Name = "顺序";

        public const string Status_Name = "状态";

        public const string Bool_True = "是";
        public const string Bool_False = "否";
    }
}
