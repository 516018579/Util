using System;
using System.IO;
using System.Reflection;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// 获取异常的内部异常详细信息
        /// </summary>
        /// <param name="exception">异常类</param>
        /// <returns>异常详细信息</returns>
        public static Exception GetInnerException(this Exception exception)
        {
            var innerEx = exception.InnerException;
            if (innerEx != null)
            {
                while (innerEx != null)
                {
                    if (innerEx.InnerException == null)
                    {
                        break;
                    }
                    innerEx = innerEx.InnerException;
                }
            }
            innerEx = innerEx ?? exception;
            return innerEx;
        }

        /// <summary>
        /// 获取内部异常的错误信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetInnerExceptionMessage(this Exception exception)
        {
            string message = exception.GetInnerException().Message;
            return message;
        }
    }
}
