using Common.Utilities;
using FluentValidation;

namespace Common.Utilities.Extensions
{
    public static class FluentValidatorExtensions
    {
        public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
            => ruleBuilder.Must(list => list.Count < num).WithMessage("The list contains too many items");

        public static IRuleBuilderOptions<T, string> FieldsRequiredAndExactLength<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName, int length)
            => ruleBuilder.FieldsMustRequired(fieldName).FieldsExactLength(fieldName, length);

        public static IRuleBuilderOptions<T, string> FieldsRequiredAndMaxLength<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName, int maxLength)
            => ruleBuilder.FieldsMustRequired(fieldName).FieldsMaxLength(fieldName, maxLength);

        public static IRuleBuilderOptions<T, string> FieldsRequiredAndWithinLength<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName, int minLength, int maxLength)
            => ruleBuilder.FieldsMustRequired(fieldName).FieldsLengthWithin(fieldName, minLength, maxLength);

        public static IRuleBuilderOptions<T, TElement> FieldsMustRequired<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder, string fieldName)
        {
            return ruleBuilder.NotEmpty()
                .WithErrorCode(ExceptionErrorCode.RequiredFieldValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.RequiredFieldValidation.GetDescription(), fieldName));
        }

        public static IRuleBuilderOptions<T, string> FieldsLengthWithin<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName, int minLength, int maxLength)
        {
            return ruleBuilder.Length(minLength, maxLength)
                .WithErrorCode(ExceptionErrorCode.FieldLengthWithtinValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.FieldLengthWithtinValidation.GetDescription(), fieldName, minLength, maxLength));
        }

        public static IRuleBuilderOptions<T, string> FieldsExactLength<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName, int length)
        {
            return ruleBuilder.Length(length)
                .WithErrorCode(ExceptionErrorCode.FieldExactLengthValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.FieldExactLengthValidation.GetDescription(), fieldName, length));
        }

        public static IRuleBuilderOptions<T, string> FieldsMaxLength<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName, int maxLength)
        {
            return ruleBuilder.MaximumLength(maxLength)
                .WithErrorCode(ExceptionErrorCode.FieldMaxLengthValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.FieldMaxLengthValidation.GetDescription(), fieldName, maxLength));
        }

        public static IRuleBuilderOptions<T, int> FieldsInclusiveBetween<T>(this IRuleBuilder<T, int> ruleBuilder, string fieldName, int minValue, int maxValue)
        {
            return ruleBuilder.InclusiveBetween(minValue, maxValue)
                .WithErrorCode(ExceptionErrorCode.ValueLengthValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.ValueLengthValidation.GetDescription(), fieldName, minValue, maxValue));
        }

        public static IRuleBuilderOptions<T, string> FieldsValidPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
        {
            return ruleBuilder.Must(BeValidE164PhoneNumber)
                .WithErrorCode(ExceptionErrorCode.FieldPhoneNumberValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.FieldPhoneNumberValidation.GetDescription(), fieldName));
        }

        public static IRuleBuilderOptions<T, string> FieldsValidDate<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
        {
            return ruleBuilder.Must(BeValidDateOnly)
                .WithErrorCode(ExceptionErrorCode.FieldDateValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.FieldDateValidation.GetDescription(), fieldName));
        }

        public static IRuleBuilderOptions<T, string> FieldsValidTime<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
        {
            return ruleBuilder.Must(BeValidTimeOnly)
                .WithErrorCode(ExceptionErrorCode.FieldTimeValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.FieldTimeValidation.GetDescription(), fieldName));
        }

        public static IRuleBuilderOptions<T, string> FieldsValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
        {
            return ruleBuilder.EmailAddress()
                .WithErrorCode(ExceptionErrorCode.FieldEmailValidation.GetAttribute<ErrorCodeAttribute>().Code)
                .WithMessage(string.Format(ExceptionErrorCode.FieldEmailValidation.GetDescription(), fieldName));
        }

        private static bool BeValidE164PhoneNumber(string phoneNumber)
        {
            string e164Pattern = @"^\+\d{1,20}$";

            return !string.IsNullOrEmpty(phoneNumber) && System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, e164Pattern);
        }

        private static bool BeValidDateOnly(string dateString) 
            => DateOnly.TryParseExact(dateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);

        private static bool BeValidTimeOnly(string timeString) 
            => TimeOnly.TryParseExact(timeString, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _);
    }
}
