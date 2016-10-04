# JSON Web Token (JWT) for .NET.

*The library supports generating and validating [JSON Web Tokens](https://tools.ietf.org/html/rfc7519).*

*See [jwt.io](https://jwt.io) for more information on JWT.*

*How to use libraries - See various examples containing in the project.*


<br/>
## JWT Token Provider

Library contanins JWT token generator based on Microsoft's implementation (System.IdentityModel.Tokens.Jwt).

### Installation [![NuGet Version](https://img.shields.io/nuget/v/Altasoft.IdentityModel.Tokens.Jwt.svg)](https://www.nuget.org/packages/Altasoft.IdentityModel.Tokens.Jwt)

> Install-Package Altasoft.IdentityModel.Tokens.Jwt

### Usage

+ *Generate:*

```C#
var jwtToken = JWTTokenProvider.GenerateToken(new JWTTokenOptions()
{
    JwtId = () => Guid.NewGuid().ToString(),
    Issuer = "altasoft",
    Audience = "any",
    IssuedAt = DateTime.Now,
    NotBefore = DateTime.Now,
    Expires = DateTime.Now.AddMinutes(60),
    Claims = claims,
    SecretKey = "1234567890123456"
});
```

+ *Validate:*
```C#
var token = JWTTokenProvider.ValidateToken(jwtTokenString, new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = JWTTokenProvider.GetSecurityKey("1234567890123456"),

    ValidateIssuer = true,
    ValidIssuer = "altasoft",

    ValidateAudience = true,
    ValidAudience = "any",

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
});
```

<br/>
## JWT Token For ASP.NET Core

### Installation [![NuGet Version](https://img.shields.io/nuget/v/Altasoft.AspNetCore.Authentication.Jwt.svg)](https://www.nuget.org/packages/Altasoft.AspNetCore.Authentication.Jwt)

> Install-Package Altasoft.AspNetCore.Authentication.Jwt

### Usage

*Startup.cs*
```C#
// Bearer:

app.UseJwtBearerAuthentication(new JwtBearerOptions
{
    AutomaticAuthenticate = true,
    AutomaticChallenge = true,
    TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = JWTTokenProvider.GetSecurityKey("1234567890123456"),

        ValidateIssuer = true,
        ValidIssuer = "altasoft",

        ValidateAudience = true,
        ValidAudience = "any",

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    }
});



// Cookie:

app.UseCookieAuthentication(new CookieAuthenticationOptions
{
    AutomaticAuthenticate = true,
    AutomaticChallenge = true,
    CookieName = "JWTCookie",
    AuthenticationScheme = "Cookie",
    SlidingExpiration = true,
    ExpireTimeSpan = TimeSpan.FromSeconds(30),
    TicketDataFormat = new JWTTicketCookieDataFormat(
        new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JWTTokenProvider.GetSecurityKey("1234567890123456"),

            ValidateIssuer = true,
            ValidIssuer = "altasoft",

            ValidateAudience = true,
            ValidAudience = "any",

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        },
        new JWTCookieOptions()
        {
            AuthenticationScheme = "Cookie",
            Issuer = "altasoft",
            Audience = "any",
            ExpiresInSeconds = 300,
            SecretKey = "1234567890123456"
        })
});
```

<br/>
*AccountController.cs*

```C#
// Cookie:

public async Task<IActionResult> Login()
{
    // todo: authenticate user

    await Request.HttpContext.Authentication.SignInAsync("Cookie"
        new ClaimsPrincipal(new ClaimsIdentity(BuildCustomClaims())),
        new AuthenticationProperties()
        {
            IssuedUtc = DateTime.UtcNow,
            ExpiresUtc = DateTime.UtcNow.AddSeconds(300)
        });

    return Ok();
}

public async Task Logout()
{
    await HttpContext.Authentication.SignOutAsync(jwtAuthConfig.Cookie.AuthenticationScheme);
}
```


<br/>
## JWT Token For ASP.NET

### Installation [![NuGet Version](https://img.shields.io/nuget/v/Altasoft.Owin.Authentication.Jwt.svg)](https://www.nuget.org/packages/Altasoft.Owin.Authentication.Jwt.Jwt)

> Install-Package Altasoft.Owin.Authentication.Jwt

### Usage
