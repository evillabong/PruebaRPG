namespace WebApi.Security
{
    public class JwtResult
    {
        public string Jti { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
