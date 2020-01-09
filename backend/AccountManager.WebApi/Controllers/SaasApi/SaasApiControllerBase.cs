using System.Web.Http;
using MediatR;

namespace AccountManager.WebApi.Controllers.SaasApi
{
    public abstract class SaasApiControllerBase : ApiController
    {
        protected const string ApiVersion = "v1";

        private readonly IMediator _mediator;

        protected SaasApiControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected IMediator Mediator => _mediator;
    }
}