namespace Altasoft.AspNetCore.Authentication.Jwt
{
    public class JWTTicketOptions
    {
        public string AuthenticationScheme { get; set; } = "Bearer";
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int? ExpiresInSeconds { get; set; }
        public string SecretKey { get; set; }
    }
}
