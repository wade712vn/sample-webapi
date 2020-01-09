using AccountManager.Application.Cloud.Queries.GetAllInstanceTypes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AccountManager.WebApi.Controllers.SaasApi
{
    [RoutePrefix("saas/api/" + ApiVersion + "/cloud")]
    public class SaasCloudApiController : SaasApiControllerBase
    {
        
        public SaasCloudApiController(IMediator mediator) : base(mediator)
        {

        }

        [HttpGet, Route("instance-types")]
        public async Task<IHttpActionResult> ListInstanceTypes()
        {
            var query = new GetAllInstanceTypesQuery();
            return Ok(await Mediator.Send(query));
        }
    }
}
