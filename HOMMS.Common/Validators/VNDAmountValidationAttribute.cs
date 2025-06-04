using System.ComponentModel.DataAnnotations;
using HOMMS.Common.Helpers;

namespace HOMMS.Common.Validators
{
    /// <summary>
    /// Validation attribute for VND amounts
    /// </summary>
    public class VNDAmountValidationAttribute : ValidationAttribute
    {
        private readonly long _minAmount;
        private readonly long _maxAmount;

        public VNDAmountValidationAttribute(long minAmount = 0, long maxAmount = 999_999_999_999)
        {
            _minAmount = minAmount;
            _maxAmount = maxAmount;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; // Let [Required] handle null validation

            if (value is not long amount)
            {
                return new ValidationResult("Amount must be a valid number.");
            }

            if (!CurrencyHelper.IsValidVNDAmount(amount))
            {
                return new ValidationResult("Invalid VND amount.");
            }

            if (amount < _minAmount)
            {
                return new ValidationResult($"Amount must be at least {CurrencyHelper.FormatVND(_minAmount)}.");
            }

            if (amount > _maxAmount)
            {
                return new ValidationResult($"Amount cannot exceed {CurrencyHelper.FormatVND(_maxAmount)}.");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validation attribute for positive VND amounts (wallet top-ups, etc.)
    /// </summary>
    public class PositiveVNDAmountAttribute : VNDAmountValidationAttribute
    {
        public PositiveVNDAmountAttribute() : base(minAmount: 1000) // Minimum 1,000 VND
        {
        }
    }

    /// <summary>
    /// Validation attribute for Vietnamese phone numbers
    /// </summary>
    public class VietnamesePhoneValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success; // Optional field

            var phone = value.ToString()!.Trim();

            // Vietnamese phone number patterns
            // Mobile: 03x, 05x, 07x, 08x, 09x (10 digits total)
            // Landline: 02x (10-11 digits total)
            var mobilePattern = @"^(03|05|07|08|09)[0-9]{8}$";
            var landlinePattern = @"^(02)[0-9]{8,9}$";

            if (System.Text.RegularExpressions.Regex.IsMatch(phone, mobilePattern) ||
                System.Text.RegularExpressions.Regex.IsMatch(phone, landlinePattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid Vietnamese phone number format.");
        }
    }
} 