using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JwtBearerAndCookieSample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using Altasoft.IdentityModel.Tokens.Jwt;

namespace JwtBearerAndCookieSample.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private JWTAuthConfig jwtAuthConfig { get; }

        public AccountController(IOptions<JWTAuthConfig> jwtConfig)
        {
            this.jwtAuthConfig = jwtConfig.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]string userName, [FromBody]string password)
        {
            await Request.HttpContext.Authentication.SignInAsync(
                jwtAuthConfig.Cookie.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(BuildCustomClaims())),
                new AuthenticationProperties()
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = jwtAuthConfig.Cookie.ExpiresInSeconds.HasValue ? DateTime.UtcNow.AddSeconds(jwtAuthConfig.Cookie.ExpiresInSeconds.Value) : default(DateTime?)
                });

            return Ok();
        }

        [HttpPost]
        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync(jwtAuthConfig.Cookie.AuthenticationScheme);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Token([FromBody]string userName, [FromBody]string password)
        {
            var claims = BuildCustomClaims();

            var now = DateTime.UtcNow;

            var jwtToken = JWTTokenProvider.GenerateToken(new JWTTokenOptions()
            {
                JwtId = () => Guid.NewGuid().ToString(),
                Audience = jwtAuthConfig.Audience,
                Issuer = jwtAuthConfig.Issuer,
                IssuedAt = now,
                NotBefore = now,
                Expires = jwtAuthConfig.ExpiresInSeconds.HasValue ? now.AddSeconds(jwtAuthConfig.ExpiresInSeconds.Value) : default(DateTime?),
                Claims = claims,
                SecretKey = jwtAuthConfig.SecretKey
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
        [Authorize]
        public IEnumerable<string> Test()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public IEnumerable<string> TestBearer()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Authorize(ActiveAuthenticationSchemes = "Cookie")]
        public IEnumerable<string> TestCookie()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
