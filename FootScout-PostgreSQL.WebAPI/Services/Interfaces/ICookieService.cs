using System.IdentityModel.Tokens.Jwt;

namespace FootScout_PostgreSQL.WebAPI.Services.Interfaces
{
    public interface ICookieService
    {
        Task SetCookies(JwtSecurityToken token, string tokenString);
    }
}