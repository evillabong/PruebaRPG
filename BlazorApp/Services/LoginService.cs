using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BlazorApp.Services
{
    public class JwtAuthenticationProvider : AuthenticationStateProvider, ILoginService
    {
        public static readonly string TokenKey = "token";
        private readonly IJSRuntime js;

        public JwtAuthenticationProvider(IJSRuntime js)
        {
            this.js = js;
        }


        private AuthenticationState Anonimo =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

            if (string.IsNullOrEmpty(token))
            {
                return Anonimo;
            }
            return await GetAuthenticationStateAsync(token);
        }

        public async Task<AuthenticationState> GetAuthenticationStateAsync(string token)
        {
            await Task.Delay(0);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity((GetClaims(token)), "jwt")));
        }

        private IEnumerable<Claim> GetClaims(string token)
        {
            var claims = new List<Claim>();
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs!.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles != null)
            {
                if (roles.ToString()!.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);

                    foreach (var parsedRole in parsedRoles!)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
            return claims;
        }

        public async Task Login(string token)
        {
            await js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            var authState = await GetAuthenticationStateAsync(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        public async Task Logout()
        {
            await js.InvokeAsync<object>("localStorage.removeItem", TokenKey);
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
        public async Task<bool> IsInRoleAsync(string roles)
        {
            return await IsInRoleAsync(roles.Split(","));
        }

        public async Task<bool> IsInRoleAsync(params string[] roles)
        {
            var authentication = await GetAuthenticationStateAsync();
            var claims = authentication.User.Claims.Where(p => p.Type.Equals("role")).FirstOrDefault();

            if (claims == null)
            {
                throw new Exception("Invalid roles.");
            }

            var rolesToken = new List<string>();
            if (claims.Value.ToString().StartsWith("["))
            {
                var jsonRoles = JsonSerializer.Deserialize<List<string>>(claims.Value);
                if (jsonRoles != null)
                {
                    rolesToken.AddRange(jsonRoles);
                }
            }
            else if (!string.IsNullOrEmpty(roles.ToString()))
            {
                if (rolesToken.Count > 0)
                {
                    rolesToken.Add(roles.ToString()!);
                }
            }

            foreach (var role in roles)
            {
                foreach (var item in rolesToken)
                {
                    if (item == role)
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        public async Task<string[]> GetRoles()
        {
            var authentication = await GetAuthenticationStateAsync();
            var claims = authentication.User.Claims.Where(p => p.Type.Equals("role")).ToList();
            return claims.Count > 0 ? claims.Select(p => p.Value).ToArray() : null!;

        }

        public async Task<string> GetJti()
        {
            var authentication = await GetAuthenticationStateAsync();
            return authentication.User.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Jti)?.Value ?? "";
        }
        public async Task<string> GetBase64Jti()
        {
            var jti = await GetJti();
            return string.IsNullOrEmpty(jti) ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(jti));
        }

        public async Task<string> GetUsername()
        {
            var state = await GetAuthenticationStateAsync();
            if (state != null)
            {
                var ret = state.User.Claims.FirstOrDefault(p => p.Type.Equals("unique_name"));
                if (ret != null)
                {
                    return ret.Value;
                }
            }
            return string.Empty;
        }

        public async Task<Dictionary<string, string>> GetClaims()
        {
            var dictionary = new Dictionary<string, string>();
            var state = await GetAuthenticationStateAsync();
            if (state != null)
            {
                var claims = state.User.Claims;
                if (claims.Count() > 0)
                {
                    foreach (var claim in claims)
                    {
                        dictionary.Add(claim.Type, claim.Value);
                    }
                    return dictionary;
                }
            }
            return dictionary;
        }

        public async Task<string> GetClaim(string key)
        {
            var claims = await GetClaims();
            if (claims.Count > 0)
            {
                return claims[key];
            }
            return string.Empty;
        }

        public async Task<bool> IsInRoleAsync<T>(params T[] roles) where T : Enum
        {
            var parse = roles.Select(p => p.ToString()).ToArray();
            return await IsInRoleAsync(parse);
        }
    }
}
