using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Runtime.CompilerServices;

namespace Common.Serilog
{
    public interface ISerilogWrapper
    {
        ILogger<SerilogWrapper> Logger { get; }

        void LogIncomingRequest([CallerMemberName] string methodName = "");

        void LogIncomingRequest(object requestContent, [CallerMemberName] string methodName = "");

        void LogOutgoingResponse(object responseContent, [CallerMemberName] string methodName = "");

        void LogException(object exceptionContent, string message, [CallerMemberName] string methodName = "");

        void LogException(Exception exception, string message, [CallerMemberName] string methodName = "");

        void LogHttpRequest(Dictionary<string, string> headers
            , string method
            , string requestUri
            , object response
            , object? request = null
            , Dictionary<string, string>? responseHeader = null);

        void LogHttpError(Dictionary<string, string> headers
            , string method
            , string requestUri
            , HttpStatusCode statusCode
            , string errorMessage
            , object? response = null
            , object? requestBody = null
            , Dictionary<string, string>? responseHeader = null);

        void LogProxyRequest(HttpContext context, string requestContent);

        void LogProxyResponse(HttpContext context, string response);

        void LogHandlerInformation(string handlerName
            , string message
            , object? content = null);

        void LogHandlerException(string handlerName, Exception ex);
    }
}
