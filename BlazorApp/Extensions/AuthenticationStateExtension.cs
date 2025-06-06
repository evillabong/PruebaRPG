using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Extensions
{
    public static class AuthenticationStateExtension
    {
        public static string GetUsername(this AuthenticationState authenticationState)
        {
            var claims = authenticationState.User.Claims.ToList();
            var claim = claims.FirstOrDefault(p => p.Type == "unique_name" || p.Type == "name");
            return claim != null ? claim.Value : "";
        }
        public static bool IsInRole(this AuthenticationState authenticationState, params string[] roles)
        {
            var claims = authenticationState.User.Claims?.Where(p => p.Type.Equals("role")).FirstOrDefault()?.Value;
            if (claims == null)
            {
                return false;
            }
            var parsedRoles = claims.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
            foreach (var role in roles)
            {
                if (parsedRoles.FirstOrDefault(p => p == role) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInRole(this AuthenticationState authenticationState, params Enum[] roles)
        {
            return IsInRole(authenticationState, roles.Select(p => p.ToString()).ToArray());
        }
    }
}
