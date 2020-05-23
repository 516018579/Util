using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Util.Domain.Attributes
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string s)
            {
                return string.IsNullOrWhiteSpace(s) || Regex.IsMatch(s, @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{7,14}$");
            }

            return true;
        }
    }
}
