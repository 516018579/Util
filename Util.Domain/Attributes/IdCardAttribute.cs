using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Util.Domain.Attributes
{
    public class IdCardAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string s)
            {
                return string.IsNullOrWhiteSpace(s) || Regex.IsMatch(s, @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$");
            }

            return true;
        }
    }
}
