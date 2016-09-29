namespace Altasoft.Owin.Authentication.Jwt
{
    public class JWTTicketOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int? ExpiresInSeconds { get; set; }
        public string SecretKey { get; set; }
    }
}
