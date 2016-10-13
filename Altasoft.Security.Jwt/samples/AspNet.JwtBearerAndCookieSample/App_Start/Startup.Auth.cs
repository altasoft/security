using Altasoft.IdentityModel.Tokens.Jwt;
using Altasoft.Owin.Authentication.Jwt;
using Altasoft.Owin.Authentication.Jwt.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace JwtBearerAndCookieSample
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var owinBearerIssuer = "LOCAL AUTHORITY";
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
            {
                AuthenticationType = OAuthDefaults.AuthenticationType,
                AllowInsecureHttp = true,
                AccessTokenFormat = new JWTTicketDataFormat(
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = owinBearerIssuer,

                        ValidateAudience = true,
                        ValidAudience = "any",

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = JWTTokenProvider.GetSecurityKey("1234567890123456"),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    },
                    new JWTTicketOptions()
                    {
                        Issuer = owinBearerIssuer,
                        Audience = "any",
                        ExpiresInSeconds = 300,
                        SecretKey = "1234567890123456"
                    })
            });


            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieName = "JWTCookie",
                CookieHttpOnly = false,
                //LoginPath = new PathString("/api/Account/Login"),
                SlidingExpiration = true,
                TicketDataFormat = new JWTTicketCookieDataFormat(
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "altasoft",

                        ValidateAudience = true,
                        ValidAudience = "any",

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = JWTTokenProvider.GetSecurityKey("1234567890123456"),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    },
                    new JWTCookieOptions()
                    {
                        Issuer = "altasoft",
                        Audience = "any",
                        ExpiresInSeconds = 300,
                        SecretKey = "1234567890123456"
                    })
            });
        }
    }
}
