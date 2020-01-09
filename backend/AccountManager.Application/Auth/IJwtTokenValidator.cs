using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Auth
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);
    }
}
