using System.ComponentModel.DataAnnotations;
using StocksApi.Web.Constants;

namespace StocksApi.Web.Validation
{
    public class ValidPriceHistoryRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !ApiConstants.PriceHistoryRangeValues.Contains(value.ToString()))
                return new ValidationResult($"The {validationContext.DisplayName} value must be one of the following: {string.Join(", ", ApiConstants.PriceHistoryRangeValues)}.",
                    new string[] { validationContext.DisplayName });

            return ValidationResult.Success;
        }
    }
}
