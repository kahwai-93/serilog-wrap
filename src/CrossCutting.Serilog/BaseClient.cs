using Common.Utilities.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Serilog
{
    public abstract class BaseService
    {
        protected readonly HttpClient _httpClient;
        protected readonly IConfiguration _configuration;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ISerilogWrapper _logger;

        private const string Accept_Language = "en";

        public JsonSerializerOptions SerializerOption { get; set; }

        public BaseService(HttpClient httpClient
            , IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor
            , ISerilogWrapper logger
            , string endpointConfigurationName)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Add(HeaderNames.AcceptLanguage, Accept_Language);

            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>(endpointConfigurationName));
            SerializerOption = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public virtual async Task<T> GetAsync<T>(string requestUri, object queryObject)
        {
            return await GetAsync<T>(requestUri + queryObject.ToQueryString());
        }

        public virtual async Task<T> GetAsync<T>(string requestUri)
        {
            var httpResponseMessage = await _httpClient.GetAsync(requestUri);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                await NonSuccessStatusCodeHandling(httpResponseMessage, "GET", requestUri);
            }

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, SerializerOption);

            await SuccessResponseHandling(httpResponseMessage, result, GetRequestHeaders(), requestUri, "GET");

            return result;
        }

        public virtual async Task<T> PostAsync<T>(string requestUri, object postObject)
        {
            var httpResponseMessage = await _httpClient.PostAsJsonAsync(requestUri, postObject);

            _logger.LogOutgoingResponse(httpResponseMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                await NonSuccessStatusCodeHandling(httpResponseMessage, "POST", requestUri, requestBody: postObject);
            }

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(responseContent, SerializerOption);

            await SuccessResponseHandling(httpResponseMessage, result, GetRequestHeaders(), requestUri, "POST", postObject);

            return result;
        }

        public virtual async Task<T> PostAsync<T>(string requestUri, FormUrlEncodedContent postObject)
        {
            var httpResponseMessage = await _httpClient.PostAsync(requestUri, postObject);

            _logger.LogOutgoingResponse(httpResponseMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                await NonSuccessStatusCodeHandling(httpResponseMessage, "POST", requestUri, requestBody: postObject);
            }

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(responseContent, SerializerOption);

            await SuccessResponseHandling(httpResponseMessage, result, GetRequestHeaders(), requestUri, "POST", postObject);

            return result;
        }

        public virtual async Task<T> PutAsync<T>(string requestUri, object putObject)
        {
            var httpResponseMessage = await _httpClient.PutAsJsonAsync(requestUri, putObject);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                await NonSuccessStatusCodeHandling(httpResponseMessage, "PUT", requestUri, requestBody: putObject);
            }

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(responseContent, SerializerOption);

            await SuccessResponseHandling(httpResponseMessage, result, GetRequestHeaders(), requestUri, "PUT", putObject);

            return result;
        }

        public virtual async Task<T> DeleteAsync<T>(string requestUri)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync(requestUri);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                await NonSuccessStatusCodeHandling(httpResponseMessage, "DELETE", requestUri);
            }

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, SerializerOption);

            await SuccessResponseHandling(httpResponseMessage, result, GetRequestHeaders(), requestUri, "DELETE");

            return result;
        }

        protected Dictionary<string, string> GetRequestHeaders() => _httpClient.DefaultRequestHeaders.ToDictionary(h => h.Key, h => string.Join(";", h.Value));

        protected Dictionary<string, string>? GetHttpResponseMessageHeaders(HttpResponseMessage? httpResponseMessage)
        {
            Dictionary<string, string>? responseHeaders = null;

            if (httpResponseMessage != null)
            {
                responseHeaders = httpResponseMessage.Headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
            }

            return responseHeaders;
        }

        protected virtual Task SuccessResponseHandling(HttpResponseMessage message
            , object? result
            , Dictionary<string, string> headers
            , string requestUri
            , string httpVerb
            , object? requestContent = null
            , Dictionary<string, string>? responseHeader = null)
        {
            //if (result is IBaseResponse)
            //{
            //    var cResult = result as IBaseResponse;

            //    if (cResult.Code != "0")
            //    {
            //        _logger.LogHttpError(headers, httpVerb, requestUri, message.StatusCode, $"{cResult.Code} : {cResult.Message}", result, requestContent);
            //        throw new Exception($"{cResult.Code} : {cResult.Message}");
            //    }
            //}

            _logger.LogHttpRequest(headers, httpVerb, requestUri, result, requestContent, responseHeader);
            return Task.CompletedTask;
        }

        protected virtual Task NonSuccessStatusCodeHandling(HttpResponseMessage httpResponseMessage, string httpVerb, string requestUri, object? response = null, object? requestBody = null)
        {
            _logger.LogHttpError(GetRequestHeaders(), httpVerb, requestUri, httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, response, requestBody);
            throw new Exception($"HTTP Client Error - {httpResponseMessage.StatusCode} : {httpResponseMessage.ReasonPhrase}");
        }
    }
}
