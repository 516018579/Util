using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Util.Domain.Attributes
{
    public class MobilePhoneAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string s)
            {
                return string.IsNullOrWhiteSpace(s) || Regex.IsMatch(s, @"^1[3456789]\d{9}$");
            }

            return true;
        }
    }
}
