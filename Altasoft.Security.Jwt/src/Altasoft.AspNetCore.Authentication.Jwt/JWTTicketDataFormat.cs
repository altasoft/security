using Altasoft.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Altasoft.AspNetCore.Authentication.Jwt
{
    public class JWTTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        protected readonly TokenValidationParameters validationParameters;
        protected readonly JWTTicketOptions ticketOptions;

        protected readonly List<string> duplicateClaims = new List<string>()
        {
            JwtRegisteredClaimNames.Jti,
            JwtRegisteredClaimNames.Aud,
            JwtRegisteredClaimNames.Iss,
            JwtRegisteredClaimNames.Iat,
            JwtRegisteredClaimNames.Nbf,
            JwtRegisteredClaimNames.Exp
        };

        public JWTTicketDataFormat(TokenValidationParameters validationParameters, JWTTicketOptions ticketOptions)
        {
            this.validationParameters = validationParameters;
            this.ticketOptions = ticketOptions;

            if (this.ticketOptions == null)
                throw new ArgumentNullException(nameof(ticketOptions));
        }

        public string Protect(AuthenticationTicket data) => Protect(data, null);

        public string Protect(AuthenticationTicket data, string purpose)
        {
            var jwtTokenOptions = BuildJwtTokenOptions(data.Principal, ticketOptions);
            var jwtCustomClaims = BuildCustomJwtClaims(data);

            var claims = data.Principal.Claims?.ToList() ?? new List<Claim>();
            claims.RemoveAll(x => duplicateClaims.Contains(x.Type));
            if (jwtCustomClaims?.Any() ?? false)
                claims.AddRange(jwtCustomClaims);

            jwtTokenOptions.Claims = claims;

            return JWTTokenProvider.GenerateToken(jwtTokenOptions);
        }

        public AuthenticationTicket Unprotect(string protectedText) => Unprotect(protectedText, null);

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            if (string.IsNullOrWhiteSpace(protectedText))
                return null;

            ClaimsPrincipal principal;
            try
            {
                var jwtTocken = JWTTokenProvider.ValidateToken(protectedText, validationParameters, out principal);
                if (jwtTocken == null)
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

            return new AuthenticationTicket(principal, RestoreAuthTicketState(principal), ticketOptions.AuthenticationScheme);
        }


        protected virtual JWTTokenOptions BuildJwtTokenOptions(ClaimsPrincipal principal, JWTTicketOptions options)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var claims = principal.Claims ?? new List<Claim>();
            var jwtTokenOptions = new JWTTokenOptions()
            {
                SecretKey = options.SecretKey,
                Claims = claims
            };


            Claim tmpClaim;

            if ((tmpClaim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                jwtTokenOptions.JwtId = () => tmpClaim.Value;
            else
                jwtTokenOptions.JwtId = () => Guid.NewGuid().ToString();

            if ((tmpClaim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                jwtTokenOptions.Audience = tmpClaim.Value;
            else
                jwtTokenOptions.Audience = options.Audience;

            if ((tmpClaim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                jwtTokenOptions.Issuer = tmpClaim.Value;
            else
                jwtTokenOptions.Issuer = options.Issuer;

            var now = DateTime.UtcNow;

            if ((tmpClaim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iat)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                jwtTokenOptions.IssuedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tmpClaim.Value));
            else
                jwtTokenOptions.IssuedAt = now;

            if ((tmpClaim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Nbf)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                jwtTokenOptions.NotBefore = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tmpClaim.Value));
            else
                jwtTokenOptions.NotBefore = now;

            if ((tmpClaim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                jwtTokenOptions.Expires = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tmpClaim.Value));
            else
                jwtTokenOptions.Expires = options.ExpiresInSeconds.HasValue ? now.AddSeconds(options.ExpiresInSeconds.Value) : default(DateTime?);


            return jwtTokenOptions;
        }

        protected virtual List<Claim> BuildCustomJwtClaims(AuthenticationTicket authTicket)
        {
            return null;
        }

        protected virtual AuthenticationProperties RestoreAuthTicketState(ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            return new AuthenticationProperties();
        }
    }
}
