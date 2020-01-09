using AccountManager.WebApi.Filters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/user")]
    public class UserApiController : ApiControllerBase
    {
        public UserApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet, Route("me")]
        public async Task<IHttpActionResult> Me()
        {
            var user = Membership.GetUser(User.Identity.Name);
            return Ok(user);
        }
    }
}
