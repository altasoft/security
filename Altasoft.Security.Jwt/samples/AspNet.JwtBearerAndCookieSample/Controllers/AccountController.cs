using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Altasoft.IdentityModel.Tokens.Jwt;

namespace JwtBearerAndCookieSample.Controllers
{
    public class AccountController : ApiController
    {
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Login()
        {
            // todo: authenticate user

            Authentication.SignIn(new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddSeconds(5),
                //IsPersistent = true
            },
            new ClaimsIdentity(BuildCustomClaims(), DefaultAuthenticationTypes.ApplicationCookie));

            return Ok();
        }


        [HttpPost]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Token()
        {
            // todo: authenticate user

            var claims = BuildCustomClaims();
            var now = DateTime.UtcNow;

            var jwtToken = JWTTokenProvider.GenerateToken(new JWTTokenOptions()
            {
                JwtId = () => Guid.NewGuid().ToString(),
                Issuer = "altasoft",
                Audience = "any",
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddMinutes(30),
                Claims = claims,
                SecretKey = "1234567890123456"
            });

            return Ok(jwtToken);
        }

        [NonAction]
        private IEnumerable<Claim> BuildCustomClaims()
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "UserName"),
                new Claim("email", "test@email.com")
            };
        }


        [HttpGet]
        public IEnumerable<string> Test()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
