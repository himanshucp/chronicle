using System.ComponentModel.DataAnnotations;

namespace Chronicle.Web
{
    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public DateRangeAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDate = value as DateTime?;

            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Property {_startDatePropertyName} not found");
            }

            var startDate = startDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (startDate.HasValue && endDate.HasValue)
            {
                if (endDate <= startDate)
                {
                    return new ValidationResult(ErrorMessage ?? "End date must be greater than start date");
                }
            }

            return ValidationResult.Success;
        }
    }
}
