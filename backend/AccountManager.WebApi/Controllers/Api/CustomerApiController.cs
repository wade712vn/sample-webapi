using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Accounts.Queries.GetAllAccounts;
using AccountManager.Application.Accounts.Queries.GetAllCustomers;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/customers")]
    public class CustomerApiController : ApiControllerBase
    {
        public CustomerApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllCustomersQuery()));
        }
    }
}
