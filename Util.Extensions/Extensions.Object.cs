using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Util.Extensions
{
    public static partial class Extensions
    {
        public static string ToString(this object value, string errorValue)
        {
            try
            {
                return value.ToString();
            }
            catch (Exception e)
            {
                return errorValue;
            }
        }

        public static T To<T>(this object obj, T defaultValue = default(T), bool catchError = true)
            where T : struct
        {
            T value = defaultValue;

            if (obj == null)
            {
                return defaultValue;
            }
            try
            {
                if (typeof(T) == typeof(Guid))
                {
                    value = (T)(object)Guid.Parse(obj.ToString());
                }

                if (typeof(T).IsEnum)
                {
                    if (Enum.IsDefined(typeof(T), obj))
                    {
                        value = (T)Enum.Parse(typeof(T), obj.ToString(), true);
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
            if (obj == null)
            {
                return defaultValue;
            }
            var value = defaultValue;
            try
            {
                value = defaultValue.HasValue ? obj.To(defaultValue.Value) : obj.To<T>(catchError: false);
            }
            catch (Exception e)
            { }

            return value;
        }

        public static object GetPropertyValue<T>(this T obj, string propertyName) where T : class
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }

        public static void SetPropertyValue<T>(this T obj, string propertyName, object value) where T : class
        {
            obj.GetType().GetProperty(propertyName)?.SetValue(obj, value);
        }
    }
}
