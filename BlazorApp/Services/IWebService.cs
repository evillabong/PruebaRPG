using Shared.Param;
using Shared.Result;

namespace BlazorApp.Services
{
    public interface IWebService
    {
        public string Token { get; set; }
        public string Url { get; set; }
        Task<TResult> GetAsync<TMethod, TResult>(TMethod endpoint) where TResult : BaseResult where TMethod : Enum;
        Task<TResult> GetAsync<TResult>(string endpoint) where TResult : BaseResult;
        Task<TResult> PostAsync<TParam, TMethod, TResult>(TMethod endpoint, TParam data) where TResult : BaseResult where TMethod : Enum where TParam : BaseParam;
        Task<LoginResult> LoginAsync(LoginParam param);
    }
}
