using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Auth.Commands;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/auth")]
    [AllowAnonymous]
    public class AuthApiController : ApiController
    {
        protected const string ApiVersion = "v1";

        private readonly IMediator _mediator;
        protected IMediator Mediator => _mediator;

        public AuthApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginCommand command)
        {
            var loginResult = await Mediator.Send(command);
            if (loginResult.Errors != null && loginResult.Errors.Any())
            {
                return Content(HttpStatusCode.Unauthorized, loginResult.Errors);
            }
            else
            {
                return Ok(new
                {
                    AccessToken = loginResult.AccessToken,
                    loginResult.RefreshToken
                });
            }
        }

        [HttpPost, Route("logout")]
        public async Task<IHttpActionResult> Logout()
        {
            return Ok();
        }
    }
}
