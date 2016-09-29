using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Altasoft.IdentityModel.Tokens.Jwt
{
    public class JWTTokenOptions
    {
        public Func<string> JwtId { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public DateTimeOffset? IssuedAt { get; set; }


        public DateTimeOffset? NotBefore { get; set; }
        public DateTimeOffset? Expires { get; set; }


        public IEnumerable<Claim> Claims { get; set; }

        public string SecretKey { get; set; }
    }
}
