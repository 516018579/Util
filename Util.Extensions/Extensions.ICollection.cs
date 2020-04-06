using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// 将对象添加到 <see cref="T:System.Collections.Generic.List`1" /> 的结尾处。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value">要添加到 <see cref="T:System.Collections.Generic.List`1" /> 末尾的对象。 如果值为 null则不添加。</param>
        public static void AddIfNotNull<T>(this ICollection<T> list, T value) where T : class
        {
            if (value != null)
            {
                list.Add(value);
            }
        }
    }
}
