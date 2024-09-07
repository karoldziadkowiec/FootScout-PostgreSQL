using FootScout_PostgreSQL.WebAPI.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace FootScout_PostgreSQL.WebAPI.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateTokenJWT(User user);
    }
}