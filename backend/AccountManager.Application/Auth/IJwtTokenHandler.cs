using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AccountManager.Application.Auth
{
    public interface IJwtTokenHandler
    {
        string WriteToken(JwtSecurityToken jwt);
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }
}