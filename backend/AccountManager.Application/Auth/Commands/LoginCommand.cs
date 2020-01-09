using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace AccountManager.Application.Auth.Commands
{
    public class LoginCommand : IRequest<LoginCommandResult>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class LoginCommandResult
    {
        public AccessToken AccessToken { get; }
        public string RefreshToken { get; }
        public IEnumerable<Error> Errors { get; }

        public LoginCommandResult(IEnumerable<Error> errors, bool success = false, string message = null)
        {
            Errors = errors;
        }

        public LoginCommandResult(AccessToken accessToken, string refreshToken, bool success = false, string message = null)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }

    public class Error
    {
        public string Code { get; }
        public string Description { get; }

        public Error(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
