namespace JwtBearerAndCookieSample.Models
{
    public class JWTAuthConfig
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int? ExpiresInSeconds { get; set; }
        public string SecretKey { get; set; }
        public JWTCookie Cookie { get; set; }

        public class JWTCookie
        {
            public string Name { get; set; }
            public string AuthenticationScheme { get; set; } = "Cookie";
            public string Domain { get; set; }
            public string Path { get; set; }
            public int? ExpiresInSeconds { get; set; }
            public int? SlidingExpirationInSeconds { get; set; }
            public bool? HttpOnly { get; set; }

            public string LoginPath { get; set; }
            public string LogoutPath { get; set; }
            public string AccessDeniedPath { get; set; }
        }
    }
}
