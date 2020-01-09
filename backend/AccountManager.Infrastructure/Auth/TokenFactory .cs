using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Auth;

namespace AccountManager.Infrastructure.Auth
{
    public sealed class TokenFactory : ITokenFactory
    {
        public string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
