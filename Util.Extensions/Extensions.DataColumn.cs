using System;
using System.Data;
using System.Globalization;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        public static void Insert(this DataColumnCollection columnCollection, string name, int index)
        {
            var colum = new DataColumn(name);
            columnCollection.Add(colum);
            colum.SetOrdinal(index);
        }

        public static void Insert<T>(this DataColumnCollection columnCollection, string name, int index, T defaultValue = default(T))
        {
            var colum = new DataColumn(name);
            if (defaultValue != null)
            {
                colum.DataType = defaultValue.GetType();
                colum.DefaultValue = defaultValue;
            }
            columnCollection.Add(colum);

            colum.SetOrdinal(index);
        }
    }
}
