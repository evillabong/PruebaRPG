using Shared.Param;
using Shared.Result;

namespace WebApi.Interfaces
{
    public interface IAuthenticationManager
    {
        Task<LoginResult> GetLoginAsync(LoginParam param);
    }
}
