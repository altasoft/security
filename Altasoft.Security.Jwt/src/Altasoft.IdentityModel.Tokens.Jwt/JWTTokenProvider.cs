using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Altasoft.IdentityModel.Tokens.Jwt
{
    public class JWTTokenProvider
    {
        public static string GenerateToken(JWTTokenOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var tokenOptions = new SecurityTokenDescriptor()
            {
                Audience = options.Audience,
                Issuer = options.Issuer,
                IssuedAt = options.IssuedAt?.LocalDateTime,
                NotBefore = options.NotBefore?.LocalDateTime,
                Expires = options.Expires?.LocalDateTime
            };

            if (options.JwtId != null)
            {
                var jwtId = options.JwtId();
                if (!string.IsNullOrWhiteSpace(jwtId))
                {
                    if (options.Claims == null)
                        options.Claims = new List<Claim>(1);

                    options.Claims = options.Claims.Concat(new Claim[] { new Claim(JwtRegisteredClaimNames.Jti, jwtId) });
                }
            }

            if (options.Claims != null && options.Claims.Any())
                tokenOptions.Subject = new ClaimsIdentity(options.Claims);

            if (!string.IsNullOrWhiteSpace(options.SecretKey))
                tokenOptions.SigningCredentials = new SigningCredentials(GetSecurityKey(options.SecretKey), SecurityAlgorithms.HmacSha256);

            return new JwtSecurityTokenHandler().CreateEncodedJwt(tokenOptions);
        }

        public static JwtSecurityToken ValidateToken(string token, TokenValidationParameters validationParameters)
        {
            ClaimsPrincipal principal;
            return ValidateToken(token, validationParameters, out principal);
        }

        public static JwtSecurityToken ValidateToken(string token, TokenValidationParameters validationParameters, out ClaimsPrincipal principal)
        {
            principal = null;

            if (string.IsNullOrWhiteSpace(token))
                return null;

            SecurityToken validatedToken = null;
            principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out validatedToken);

            return validatedToken as JwtSecurityToken;
        }

        public static SymmetricSecurityKey GetSecurityKey(string secretKey)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentNullException(nameof(secretKey));

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }
    }
}
