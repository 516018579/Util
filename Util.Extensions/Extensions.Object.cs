using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        public static bool IsNullOrWhiteSpace(this object value)
        {
            return value == null || string.IsNullOrWhiteSpace(value.ToString());
        }

        public static bool IsNotNullOrWhiteSpace(this object value)
        {
            return !value.IsNullOrWhiteSpace();
        }

        public static T To<T>(this object obj, T defaultValue = default(T), bool catchError = true)
            where T : struct
        {
            T value = defaultValue;
            try
            {
                if (typeof(T) == typeof(Guid))
                {
                    value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
                }

                if (typeof(T).IsEnum)
                {
                    if (Enum.IsDefined(typeof(T), obj))
                    {
                        value = (T)Enum.Parse(typeof(T), obj.ToString());
                    }
                    else
                    {
                        throw new ArgumentException($"Enum type undefined '{obj}'.");
                    }
                }

                value = (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                if (!catchError)
                {
                    throw;
                }
            }

            return value;
        }

        public static T? ToNullable<T>(this object obj, T? defaultValue = default(T?))
            where T : struct
        {
            var value = defaultValue;
            try
            {
                value = defaultValue.HasValue ? obj.To(defaultValue.Value) : obj.To<T>(catchError: false);
            }
            catch (Exception e)
            { }

            return value;
        }
    }
}
