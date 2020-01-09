using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;
using MediatR;

namespace AccountManager.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;

        public LoginCommandHandler(IJwtFactory jwtFactory, ITokenFactory tokenFactory)
        {
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
        }

        public async Task<LoginCommandResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            if (Membership.ValidateUser(command.Username, command.Password))
            {
                var refreshToken = _tokenFactory.GenerateToken();

                var user = Membership.GetUser(command.Username);
                if (user == null)
                {
                    return new LoginCommandResult(new[] { new Error("login_failure", "Error occurred getting account information.") });
                }

                return new LoginCommandResult(await _jwtFactory.GenerateEncodedToken(user.ProviderUserKey as string, user.UserName),
                    refreshToken, true);
            }

            return new LoginCommandResult(new[] { new Error("login_failure", "Invalid username or password.") });
        }
    }
}
