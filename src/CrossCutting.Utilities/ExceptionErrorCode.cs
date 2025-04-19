using System.ComponentModel;
using System.Resources;

namespace Common.Utilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorCodeAttribute : Attribute
    {
        public string Code { get; }

        public ErrorCodeAttribute(string code)
        {
            Code = code;
        }
    }

    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceKey;
        private readonly ResourceManager _resource;
        public LocalizedDescriptionAttribute(string resourceKey, Type resourceType)
        {
            _resource = new ResourceManager(resourceType);
            _resourceKey = resourceKey;
        }

        public override string Description
        {
            get
            {
                string displayName = _resource.GetString(_resourceKey);

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("{0}", _resourceKey)
                    : displayName;
            }
        }
    }

    public enum ExceptionErrorCode
    {
        [ErrorCode("9001")]
        [LocalizedDescription("{0} is required.", typeof(ExceptionResources))]
        RequiredFieldValidation,

        [ErrorCode("9002")]
        [LocalizedDescription("{0} length must not exceed {1}.", typeof(ExceptionResources))]
        FieldMaxLengthValidation,

        [ErrorCode("9003")]
        [LocalizedDescription("{0} must be between {1} and {2}.", typeof(ExceptionResources))]
        ValueLengthValidation,

        [ErrorCode("9004")]
        [LocalizedDescription("{0} length must be exact {1}.", typeof(ExceptionResources))]
        FieldExactLengthValidation,

        [ErrorCode("9005")]
        [LocalizedDescription("{0} must be in E.164 format.", typeof(ExceptionResources))]
        FieldPhoneNumberValidation,

        [ErrorCode("9006")]
        [LocalizedDescription("{0} must not less than {1}.", typeof(ExceptionResources))]
        FieldMinValueValidation,

        [ErrorCode("9007")]
        [LocalizedDescription("{0} must be in yyyy-MM-dd format.", typeof(ExceptionResources))]
        FieldDateValidation,

        [ErrorCode("9008")]
        [LocalizedDescription("Invalid Identity Type.", typeof(ExceptionResources))]
        FieldIdentityTypeValidation,

        [ErrorCode("9009")]
        [LocalizedDescription("Invalid email format for {0}.", typeof(ExceptionResources))]
        FieldEmailValidation,

        [ErrorCode("9010")]
        [LocalizedDescription("{0} must be in HH:mm:ss format.", typeof(ExceptionResources))]
        FieldTimeValidation,

        [ErrorCode("9011")]
        [LocalizedDescription("Invalid State Code.", typeof(ExceptionResources))]
        StateValidation,

        [ErrorCode("9012")]
        [LocalizedDescription("Invalid Country Code.", typeof(ExceptionResources))]
        CountryCodeValidation,

        [ErrorCode("9013")]
        [LocalizedDescription("{0} length must be minimum {1} to maximum {2}.", typeof(ExceptionResources))]
        FieldLengthWithtinValidation,

        [ErrorCode("9014")]
        [LocalizedDescription("End date must after Start date.", typeof(ExceptionResources))]
        EndDateGreaterThanStartDateValidation,

        [ErrorCode("9015")]
        [LocalizedDescription("End date and Start Date must be within {0} days.", typeof(ExceptionResources))]
        DateRangeValidation,

        [ErrorCode("9016")]
        [LocalizedDescription("Identity No. cannot be 'NA'.", typeof(ExceptionResources))]
        FieldIdentityNoValidation,

        [ErrorCode("900")]
        [LocalizedDescription("Invalid Request Content", typeof(ExceptionResources))]
        InvalidRequestContent,

        [ErrorCode("901")]
        [LocalizedDescription("HTTP Client Error", typeof(ExceptionResources))]
        HTTPClientError,

        [ErrorCode("999")]
        [LocalizedDescription("Error Occurred", typeof(ExceptionResources))]
        GeneralError
    }
}
