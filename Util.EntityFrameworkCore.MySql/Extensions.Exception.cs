using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Util.EntityFrameworkCore.Mysql
{
    public static partial class EntityFrameworkCoreMysqlExtensions
    {
        /// <summary>
        /// 是否是主键冲突异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsDuplicateKeyException(this DbUpdateException ex)
        {
            return ex.GetSqlExceptionNumber() == 1062 && ex.InnerException?.Message?.Contains("PRIMARY") == true;
        }

        /// <summary>
        /// 是否唯一约束冲突一次
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsUniqueKeyException(this DbUpdateException ex)
        {
            return ex.GetSqlExceptionNumber() == 2601 && ex.InnerException?.Message?.Contains("PRIMARY") != true;
        }

        /// <summary>
        /// 是否外键冲突
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsForeignKeyException(this DbUpdateException ex)
        {
            var number = ex.GetSqlExceptionNumber();
            return number == 1452 || number == 1451;
        }


        private static int? GetSqlExceptionNumber(this DbUpdateException ex)
        {
            var innerException = ex.InnerException;
            if (innerException is MySqlException sqlException)
            {
                return sqlException.Number;
            }

            return null;
        }
    }
}
