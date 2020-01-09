using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Auth
{
    public class AccessToken
    {
        public string Token { get; }
        public int ExpiresIn { get; }

        public AccessToken(string token, int expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
        }
    }
}
