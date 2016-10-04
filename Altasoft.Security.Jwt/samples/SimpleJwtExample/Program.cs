using Altasoft.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleJwtExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Check JWT Token at https://jwt.io");
            Console.WriteLine();

            Console.WriteLine("Choose the letter for action: [G]-Generate jwt, [V]-Validate jwt");
            var key = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (key.ToString().ToLower() == "g")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "usr"),
                    new Claim("mail", "test@mail.com")
                };

                var now = DateTime.Now;

                var jwtToken = JWTTokenProvider.GenerateToken(new JWTTokenOptions()
                {
                    JwtId = () => Guid.NewGuid().ToString(),
                    Issuer = "altasoft",
                    Audience = "any",
                    IssuedAt = now,
                    NotBefore = now,
                    Expires = now.AddMinutes(5),
                    Claims = claims,
                    SecretKey = "1234567890123456"
                });

                Console.WriteLine(jwtToken);
            }
            else if (key.ToString().ToLower() == "v")
            {
                Console.WriteLine("JWT Token string:");
                var jwtString = Console.ReadLine();

                Console.WriteLine("JWT Token secret key:");
                var jwtSecretKey = Console.ReadLine();

                try
                {
                    var token = JWTTokenProvider.ValidateToken(jwtString, new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = JWTTokenProvider.GetSecurityKey(jwtSecretKey),

                        ValidateIssuer = true,
                        ValidIssuer = "altasoft",

                        ValidateAudience = true,
                        ValidAudience = "any",

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                    });

                    Console.WriteLine("JWT Token is valid");
                }
                catch (SecurityTokenException ex)
                {
                    Console.WriteLine("JWT Token is not valid");
                    Console.WriteLine();
                    Console.WriteLine(ex);
                }
            }
            else
                Console.WriteLine("Program will exit");

            Console.ReadLine();
        }
    }
}
