namespace Common.Utilities
{
    public interface IHandledException
    {
        public string Code { get; }
    }

    public class BaseHandledException : Exception, IHandledException
    {
        protected string _code;
        public string Code { get => _code; }

        public BaseHandledException(string errorCode, string message) : base(message)
        {
            _code = errorCode;
        }
    }
}
