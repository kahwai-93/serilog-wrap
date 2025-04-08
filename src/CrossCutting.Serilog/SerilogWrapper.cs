using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.CompilerServices;

namespace Common.Serilog
{
    public class SerilogWrapper : ISerilogWrapper
    {
        public ILogger<SerilogWrapper> Logger
        {
            get
            {
                return _logger;
            }
        }

        private readonly ILogger<SerilogWrapper> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string IncomingRequestTemplate = "Request Logging for {MethodName}";
        private const string OutgoingResponseTemplate = "Response Logging for {MethodName}";
        private const string ErrorTemplate = "Exception Occurred {MethodName} : {ErrorMessage}";
        private const string HttpRequestTemplate = "HTTP {Method} Success {URI}";
        private const string HttpErrorTemplate = "HTTP {Method} Error {HttpStatus} {URI} - {ErrorMessage}";
        private const string ProxyRequestTemplate = "Custom Proxy Request {HttpVerb} {URI}";
        private const string ProxyResponseTemplate = "Custom Proxy Response {HttpVerb} {URI} {StatusCode}";
        private const string HandlerErrorTemplate = "IRequestHandler {handlerName} Error : {errorMessage}";
        private const string HandlerMessageTemplate = "IRequestHandler {handlerName} Information : {informationMessage}";

        public SerilogWrapper(ILogger<SerilogWrapper> logger
            , IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogIncomingRequest([CallerMemberName] string methodName = "")
        {
            using (_logger.BeginScope(new Dictionary<string, object> {
                { "RequestHeaders", GetHttpHeaders()}
                }))
            {
                _logger.LogInformation(IncomingRequestTemplate, methodName);
            }
        }

        public void LogIncomingRequest(object requestContent, [CallerMemberName] string methodName = "")
        {
            using (_logger.BeginScope(new Dictionary<string, object> {
                { "@RequestContent", requestContent },
                { "RequestHeaders", GetHttpHeaders()}
                }))
            {
                _logger.LogInformation(IncomingRequestTemplate, methodName);
            }
        }

        public void LogOutgoingResponse(object responseContent, [CallerMemberName] string methodName = "")
        {
            using (_logger.BeginScope(new Dictionary<string, object> {
                { "@ResponseContent", responseContent },
                { "RequestHeaders", GetHttpHeaders()}
                }))
            {
                _logger.LogInformation(OutgoingResponseTemplate, methodName);
            }
        }

        public void LogException(object exceptionContent, string message, [CallerMemberName] string methodName = "")
        {
            using (_logger.BeginScope(new Dictionary<string, object> {
                { "@ErrorResponse", exceptionContent },
                { "RequestHeaders", GetHttpHeaders() }
                }))
            {
                _logger.LogError(ErrorTemplate, methodName, message);
            }
        }

        public void LogException(Exception exception, string message, [CallerMemberName] string methodName = "")
        {
            using (_logger.BeginScope(new Dictionary<string, object> {
                { "RequestHeaders", GetHttpHeaders() }
                }))
            {
                _logger.LogError(exception, ErrorTemplate, methodName, message);
            }
        }

        public void LogHttpRequest(Dictionary<string, string> headers
            , string method
            , string requestUri
            , object response
            , object? request = null
            , Dictionary<string, string>? responseHeader = null)
        {
            var extraProperties = new Dictionary<string, object> {
                { "OutgoingHeaders", headers},
                { "@ResponseContent", response }
                };

            if (request != null)
            {
                extraProperties.Add("@RequestContent", request);
            }

            if (responseHeader != null)
            {
                extraProperties.Add("@ResponseHeader", responseHeader);
            }

            using (_logger.BeginScope(extraProperties))
            {
                _logger.LogInformation(HttpRequestTemplate, method, requestUri);
            }
        }

        public void LogHttpError(Dictionary<string, string> headers
            , string method
            , string requestUri
            , HttpStatusCode statusCode
            , string errorMessage
            , object? response = null
            , object? requestBody = null
            , Dictionary<string, string>? responseHeader = null)
        {
            var extraProperties = new Dictionary<string, object> {
                { "OutgoingHeaders", headers}
            };

            if (response != null)
            {
                extraProperties.Add("@ErrorContent", response);
            }

            if (requestBody != null)
            {
                extraProperties.Add("@RequestContent", requestBody);
            }

            if (responseHeader != null)
            {
                extraProperties.Add("@ResponseHeader", responseHeader);
            }

            using (_logger.BeginScope(extraProperties))
            {
                _logger.LogInformation(HttpErrorTemplate, method, statusCode, requestUri, errorMessage);
            }
        }

        public void LogProxyRequest(HttpContext context, string requestContent)
        {
            try
            {
                object obj;

                if (!string.IsNullOrEmpty(context.Request.ContentType) && context.Request.ContentType.Contains("application/json"))
                {
                    obj = JsonConvert.DeserializeObject<dynamic>(requestContent);
                }
                else
                {
                    obj = requestContent;
                }

                var extraProperties = new Dictionary<string, object> {
                    { "IncomingHeaders", context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())},
                    { "@RequestContent", obj }
                };

                using (_logger.BeginScope(extraProperties))
                {
                    _logger.LogInformation(ProxyRequestTemplate
                        , context.Request.Method
                        , context.Request.GetDisplayUrl());
                }
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object> {
                    { "RequestHeaders", GetHttpHeaders() },
                    { "Exception", ex.ToString()},
                    { "ExceptionRequestContent", requestContent}
                }))
                {
                    _logger.LogError(ErrorTemplate, "LogProxyRequest", ex.Message);
                }
            }
        }

        public void LogProxyResponse(HttpContext context, string response)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(response);
                var extraProperties = new Dictionary<string, object> {
                    { "IncomingHeaders", context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())},
                    { "@ResponseContent", obj },
                    { "ResponseStatusCode", context.Response.StatusCode }
                };

                using (_logger.BeginScope(extraProperties))
                {
                    _logger.LogInformation(ProxyResponseTemplate
                        , context.Request.Method
                        , context.Request.GetDisplayUrl()
                        , context.Response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object> {
                    { "RequestHeaders", GetHttpHeaders() },
                    { "Exception", ex.ToString()}
                }))
                {
                    _logger.LogError(HandlerErrorTemplate, "LogProxyResponse", ex.Message);
                }
            }
        }

        public void LogHandlerInformation(string handlerName
            , string message
            , object? content = null)
        {
            var extraProperties = new Dictionary<string, object> {
                { "RequestHeaders", GetHttpHeaders() },
                { "HandlerName", handlerName},
            };

            if (content != null)
            {
                extraProperties.Add("@HandlerLogContent", content);
            }

            using (_logger.BeginScope(extraProperties))
            {
                _logger.LogInformation(HandlerMessageTemplate, handlerName, message);
            }
        }

        public void LogHandlerException(string handlerName, Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object> {
                    { "RequestHeaders", GetHttpHeaders() },
                    { "HandlerName", handlerName},
                    { "Exception", ex.ToString()}
                }))
            {
                _logger.LogError(HandlerErrorTemplate, handlerName, ex.Message);
            }
        }

        private Dictionary<string, string> GetHttpHeaders() => _httpContextAccessor?.HttpContext?.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()) ?? new Dictionary<string, string>();
    }
}
