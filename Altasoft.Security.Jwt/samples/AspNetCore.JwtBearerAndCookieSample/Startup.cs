using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JwtBearerAndCookieSample.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Altasoft.IdentityModel.Tokens.Jwt;
using Altasoft.AspNetCore.Authentication.Jwt.Cookies;

namespace JwtBearerAndCookieSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.Configure<JWTAuthConfig>(Configuration.GetSection("JWT"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<JWTAuthConfig> jwtConfigOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            #region Auth

            var jwtAuthConfig = jwtConfigOptions.Value;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = JWTTokenProvider.GetSecurityKey(jwtAuthConfig.SecretKey),

                ValidateIssuer = true,
                ValidIssuer = jwtAuthConfig.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtAuthConfig.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };


            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,
                //Events = new JwtBearerEvents()
                //{
                //    OnTokenValidated = ctx =>
                //    {
                //        // do extra validation
                //        if (!ctx.Ticket.Principal.HasClaim(x => x.Type == "IpAddress"))
                //        {
                //            ctx.SkipToNextMiddleware();
                //        }

                //        return Task.CompletedTask;
                //    }
                //}
            });

            //Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                CookieName = jwtAuthConfig.Cookie.Name,
                AuthenticationScheme = jwtAuthConfig.Cookie.AuthenticationScheme,
                CookieDomain = jwtAuthConfig.Cookie.Domain,
                CookiePath = jwtAuthConfig.Cookie.Path,
                CookieHttpOnly = jwtAuthConfig.Cookie.HttpOnly.GetValueOrDefault(),
                SlidingExpiration = jwtAuthConfig.Cookie.SlidingExpirationInSeconds.HasValue,
                ExpireTimeSpan = TimeSpan.FromSeconds(jwtAuthConfig.Cookie.SlidingExpirationInSeconds.GetValueOrDefault()),
                TicketDataFormat = new JWTTicketCookieDataFormat(tokenValidationParameters, new JWTCookieOptions()
                {
                    AuthenticationScheme = jwtAuthConfig.Cookie.AuthenticationScheme,
                    Audience = jwtAuthConfig.Audience,
                    Issuer = jwtAuthConfig.Issuer,
                    ExpiresInSeconds = jwtAuthConfig.ExpiresInSeconds,
                    SecretKey = jwtAuthConfig.SecretKey
                }),
                LoginPath = jwtAuthConfig.Cookie.LoginPath,
                LogoutPath = jwtAuthConfig.Cookie.LogoutPath,
                AccessDeniedPath = jwtAuthConfig.Cookie.AccessDeniedPath

                //Events = new CookieAuthenticationEvents()
                //{
                //    OnValidatePrincipal = async ctx =>
                //    {
                //        // do extra validation
                //        if (!ctx.Principal.HasClaim(x => x.Type == "IpAddress"))
                //        {
                //            ctx.RejectPrincipal();
                //            await ctx.HttpContext.Authentication.SignOutAsync(ctx.Options.AuthenticationScheme);
                //        }
                //    }
                //}
            });

            #endregion

            app.UseMvc();
        }
    }
}
