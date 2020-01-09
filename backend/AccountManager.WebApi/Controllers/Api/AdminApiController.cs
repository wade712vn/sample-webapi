using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/admin")]
    public class AdminApiController : ApiControllerBase
    {
        public AdminApiController(IMediator mediator) : base(mediator)
        {
        }
    }
}
