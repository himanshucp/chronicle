using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Chronicle.Web
{
    /// <summary>
    /// Validates international phone and fax numbers using E.164 format.
    /// Example of valid format: +12345678900
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InternationalPhoneAttribute : ValidationAttribute
    {
        // E.164 format: + followed by up to 15 digits
        private const string E164Pattern = @"^\+?[1-9]\d{1,14}$";

        public override bool IsValid(object value)
        {
            if (value is null) return true; // Not required by default

            if (value is string phone)
            {
                if (string.IsNullOrWhiteSpace(phone)) return true;

                return Regex.IsMatch(phone, E164Pattern);
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field is not a valid international phone number (e.g., +12345678900).";
        }
    }
}
