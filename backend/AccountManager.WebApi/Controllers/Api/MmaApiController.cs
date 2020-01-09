using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Cloud.Queries.GetAllRegions;
using AccountManager.Application.Mma.Queries.GetAllClasses;
using AccountManager.Application.Mma.Queries.GetAllOperationTypes;
using AccountManager.Application.Queries.GetAllClasses;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/mma")]
    public class MmaApiController : ApiControllerBase
    {
        public MmaApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet, Route("classes")]
        public async Task<IHttpActionResult> GetAllClasses()
        {
            return Ok(await Mediator.Send(new GetAllClassesQuery()));
        }

        [HttpGet, Route("operation-types")]
        public async Task<IHttpActionResult> GetAllOperationTypes()
        {
            return Ok(await Mediator.Send(new GetAllOperationTypesQuery()));
        }
    }
}
