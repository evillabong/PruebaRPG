using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Shared.Param;
using Shared.Result;
using Shared.Types;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace BlazorApp.Services
{
    public class WebService : IWebService
    {
        IJSRuntime _js;
        private HttpClient _httpClient = new();
        public string Token { get; set; } = string.Empty;
        public string Url { get; set; }

        private readonly JsonSerializerOptions _jsonOptions;
        public WebService(IJSRuntime js, HttpClient httpClient, string url)
        {
            this._js = js;
            this._httpClient = httpClient;
            this.Url = url;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
            };
        }
        public async Task<TResult> GetAsync<TMethod, TResult>(TMethod endpoint) where TResult : BaseResult where TMethod : Enum
        {
            var url = _httpClient.BaseAddress!.ToString();
            url += $"api/{endpoint.GetType().Name}/{endpoint.ToString()}";
            return await GetAsync<TResult>(url);
        }

        public async Task<TResult> GetAsync<TResult>(string endpoint) where TResult : BaseResult
        {
            await SetAuthorizations();
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(endpoint),

            };
            return await SendRequest<TResult>(requestMessage);
        }

        public async Task<TResult> PostAsync<TParam, TMethod, TResult>(TMethod endpoint, TParam data) where TResult : BaseResult where TMethod : Enum where TParam : BaseParam
        {
            await SetAuthorizations();

            //var url = $"{await GetUrl()}/api/{method.GetType().Name}/{method.ToString()}{query?.GetQueryString()}";
            var url = _httpClient.BaseAddress!.ToString();
            url += $"api/{endpoint.GetType().Name}/{endpoint.ToString()}";
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = CreateJsonContent(data),
            };

            return await SendRequest<TResult>(requestMessage);
        }


        private async Task SetAuthorizations()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");
            this.Token = token;
        }

        private StringContent CreateJsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<TResult> SendRequest<TResult>(HttpRequestMessage requestMessage) where TResult : BaseResult
        {
            requestMessage.SetBrowserRequestCache(BrowserRequestCache.NoCache);
            requestMessage.SetBrowserRequestMode(BrowserRequestMode.Cors);
            requestMessage.SetBrowserRequestCredentials(BrowserRequestCredentials.Omit);

            if (!string.IsNullOrEmpty(Token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            }

            var response = await _httpClient.SendAsync(requestMessage);
            var result = await GetResult<TResult>(response);

            return result;
        }

        private async Task<TResult> GetResult<TResult>(HttpResponseMessage responseMessage) where TResult : BaseResult
        {
            System.Type type = typeof(TResult);
            var result = (TResult)Activator.CreateInstance(type)!;

            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<TResult>(content , _jsonOptions);

                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    result.ResultCode = (int)ResultType.SERVER_INTERNAL_ERROR;
                    break;
                case System.Net.HttpStatusCode.NotFound:
                    result.ResultCode = (int)ResultType.SERVER_NOT_FOUND;
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    result.ResultCode = (int)ResultType.SERVER_UNAUTHORIZED;
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    result.ResultCode = (int)ResultType.SERVER_FORBIDDEN;
                    break;
                case System.Net.HttpStatusCode.BadRequest: // falla decriptar
                    result.ResultCode = (int)ResultType.SERVER_BAD_REQUEST;
                    break;
                default:
                    result.ResultCode = (int)ResultType.SERVER_FAIL;
                    result.Message = await responseMessage.Content.ReadAsStringAsync();
                    break;

            }
            return result;
        }

        public async Task<LoginResult> LoginAsync(LoginParam param)
        {
            var response = await PostAsync<LoginParam, Shared.WebMethods.Security, LoginResult>(Shared.WebMethods.Security.Login, param);
            if (response.ResultCode == (int)ResultType.Success)
            {
                this.Token = response.Token!;
            }
            return response;
        }
        private string GetQueryString(Dictionary<string, string>? query)
        {
            if (query != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var item in query)
                {
                    if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        if (builder.Length == 0)
                        {
                            builder.Append($"{item.Key}={item.Value}");
                        }
                        else
                        {
                            builder.Append($"&{item.Key}={item.Value}");
                        }
                    }
                }
                if (builder.Length != 0)
                {
                    return $"?{builder.ToString()}";
                }
            }
            return string.Empty;
        }
    }
}
