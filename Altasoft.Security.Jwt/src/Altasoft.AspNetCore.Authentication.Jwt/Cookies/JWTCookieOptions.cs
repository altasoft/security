namespace Altasoft.AspNetCore.Authentication.Jwt.Cookies
{
    public class JWTCookieOptions : JWTTicketOptions
    {
        public JWTCookieOptions()
        {
            base.AuthenticationScheme = "Cookie";
        }
    }
}
