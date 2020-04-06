using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Util.EntityFrameworkCore.Sqlserver
{
    public static partial class EntityFrameworkCoreSqlserverExtensions
    {
        /// <summary>
        /// 是否是主键冲突异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsDuplicateKeyException(this DbUpdateException ex)
        {
            return ex.GetSqlExceptionNumber() == 2627;
        }

        /// <summary>
        /// 是否唯一约束冲突一次
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsUniqueKeyException(this DbUpdateException ex)
        {
            return ex.GetSqlExceptionNumber() == 2601;
        }

        /// <summary>
        /// 是否外键冲突
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsForeignKeyException(this DbUpdateException ex)
        {
            return ex.GetSqlExceptionNumber() == 547;
        }


        private static int? GetSqlExceptionNumber(this DbUpdateException ex)
        {
            var innerException = ex.InnerException;
            if (innerException is SqlException sqlException)
            {
                return sqlException.Number;
            }

            return null;
        }
    }
}
