using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AccountManager.WebApi.Filters
{
    public class SaasBasicAuthenticationFilter : BasicAuthenticationFilter
    {
        public SaasBasicAuthenticationFilter(IMediator mediator)
        {

        }

        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {



            var identity = new ClaimsIdentity();
            return new ClaimsPrincipal(identity);
        }
    }
}