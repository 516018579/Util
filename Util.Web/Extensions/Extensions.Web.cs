using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Util.Web.Extensions
{
    public static class WebExtensions
    {
        #region IFormFile
        public static Task SaveAsync(this IFormFile file, string path)
        {
            using (var fileStream = new FileStream(Path.Combine(WebConsts.WebRootPath, path), FileMode.Create))
            {
                return file.CopyToAsync(fileStream);
            }
        }
        #endregion

        #region ISession
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
        #endregion
    }
}
