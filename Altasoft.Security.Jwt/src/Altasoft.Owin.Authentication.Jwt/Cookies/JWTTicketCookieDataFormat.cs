using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Altasoft.Owin.Authentication.Jwt.Cookies
{
    public class JWTTicketCookieDataFormat : JWTTicketDataFormat
    {
        public JWTTicketCookieDataFormat(TokenValidationParameters validationParameters, JWTCookieOptions cookieOptions)
            : base(validationParameters, cookieOptions)
        {
            base.duplicateClaims.AddRange(new List<string>()
            {
                JWTCookieClaimNames.AllowRefresh,
                JWTCookieClaimNames.IsPersistent,
                JWTCookieClaimNames.IssuedUtc,
                JWTCookieClaimNames.ExpiresUtc
            });
        }

        protected override List<Claim> BuildCustomJwtClaims(AuthenticationTicket authTicket)
        {
            if (authTicket?.Properties == null)
                return null;

            var claims = base.BuildCustomJwtClaims(authTicket) ?? new List<Claim>(4);
            var authProperties = authTicket.Properties;

            if (authProperties.AllowRefresh.HasValue)
                claims.Add(new Claim(JWTCookieClaimNames.AllowRefresh, authProperties.AllowRefresh.Value.ToString()));

            if (authProperties.IsPersistent)
                claims.Add(new Claim(JWTCookieClaimNames.IsPersistent, Convert.ToInt32(authProperties.IsPersistent).ToString()));

            if (authProperties.IssuedUtc.HasValue)
                claims.Add(new Claim(JWTCookieClaimNames.IssuedUtc, authProperties.IssuedUtc.Value.ToUnixTimeSeconds().ToString()));

            if (authProperties.ExpiresUtc.HasValue)
                claims.Add(new Claim(JWTCookieClaimNames.ExpiresUtc, authProperties.ExpiresUtc.Value.ToUnixTimeSeconds().ToString()));

            return claims;
        }

        protected override AuthenticationProperties RestoreAuthTicketState(ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var authTicketProps = base.RestoreAuthTicketState(principal);
            Claim tmpClaim;

            if ((tmpClaim = principal.Claims.FirstOrDefault(x => x.Type == JWTCookieClaimNames.AllowRefresh)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                authTicketProps.AllowRefresh = bool.Parse(tmpClaim.Value);

            if ((tmpClaim = principal.Claims.FirstOrDefault(x => x.Type == JWTCookieClaimNames.IsPersistent)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                authTicketProps.IsPersistent = bool.Parse(tmpClaim.Value);

            if ((tmpClaim = principal.Claims.FirstOrDefault(x => x.Type == JWTCookieClaimNames.IssuedUtc)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                authTicketProps.IssuedUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tmpClaim.Value));

            if ((tmpClaim = principal.Claims.FirstOrDefault(x => x.Type == JWTCookieClaimNames.ExpiresUtc)) != null && !string.IsNullOrWhiteSpace(tmpClaim.Value))
                authTicketProps.ExpiresUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tmpClaim.Value));

            return authTicketProps;
        }
    }
}
