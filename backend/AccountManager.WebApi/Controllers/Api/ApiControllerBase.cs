using System.Web.Http;
using AccountManager.WebApi.Filters;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [JwtAuthenticationFilter()]
    public abstract class ApiControllerBase : ApiController
    {
        protected const string ApiVersion = "v1";

        private readonly IMediator _mediator;

        protected ApiControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected IMediator Mediator => _mediator;
    }
}