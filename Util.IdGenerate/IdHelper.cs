using System;
using Snowflake.Core;

namespace Util.IdGenerate
{
    public class IdHelper
    {
        private static readonly IdWorker Worker = new IdWorker(1, 1);

        public static long GenerateId()
        {
            return Worker.NextId();
        }

        public static string GenerateStringId()
        {
            return GenerateId().ToString();
        }

        public static string GenerateDateStringId(string format = "yyyyMMdd")
        {
            return DateTime.Now.ToString(format) + GenerateId();
        }
    }
}
