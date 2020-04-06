using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        public static string GetPath(this Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }

    }
}
