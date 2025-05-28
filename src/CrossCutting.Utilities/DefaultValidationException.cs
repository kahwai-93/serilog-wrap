namespace Common.Utilities
{
    public class DefaultValidationException : BaseHandledException
    {
        private const string DefaultMessage = "Validation Failed.";

        public DefaultValidationException(string errorCode) : base(errorCode, DefaultMessage) { }

        public DefaultValidationException(string errorCode, string message) : base(errorCode, message) { }
    }
}
