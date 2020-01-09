using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AccountManager.Application.Cloud.Queries.GetAllImages;
using AccountManager.Application.Cloud.Queries.GetAllInstanceTypes;
using AccountManager.Application.Cloud.Queries.GetAllRegions;
using AccountManager.WebApi.Filters;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    
    [RoutePrefix("api/" + ApiVersion + "/cloud")]
    public class CloudApiController : ApiControllerBase
    {
        public CloudApiController(IMediator mediator) : base(mediator)
        {
            
        }

        [HttpGet, Route("regions")]
        public async Task<IHttpActionResult> GetAllRegions()
        {
            return Ok(await Mediator.Send(new GetAllRegionsQuery()));
        }

        [HttpGet, Route("instance-types")]
        public async Task<IHttpActionResult> GetAllInstanceTypes()
        {
            return Ok(await Mediator.Send(new GetAllInstanceTypesQuery()));
        }

        [HttpGet, Route("images")]
        public async Task<IHttpActionResult> GetAllImages()
        {
            return Ok(await Mediator.Send(new GetAllImagesQuery()));
        }
    }
}