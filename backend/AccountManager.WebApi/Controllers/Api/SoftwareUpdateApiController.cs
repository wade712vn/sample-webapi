using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/software-update")]
    public class SoftwareUpdateApiController : ApiControllerBase
    {
        public SoftwareUpdateApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Commands

        [HttpPost, Route("update-machines")]
        public async Task<IHttpActionResult> UpdateMachines([FromBody] UpdateSoftwareForMachinesCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Route("update-accounts")]
        public async Task<IHttpActionResult> UpdateAccounts([FromBody] UpdateSoftwareForAccountsCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        #endregion
    }
}
