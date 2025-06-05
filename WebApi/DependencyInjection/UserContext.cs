using Model.Interfaces;

namespace WebApi.DependencyInjection
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetUsername()
        {
            return httpContextAccessor.HttpContext!.User.Identity!.Name!;
        }
    }
}
