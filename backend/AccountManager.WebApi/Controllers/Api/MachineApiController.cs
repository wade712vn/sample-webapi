using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Accounts.Commands.BatchUpdateMachines;
using AccountManager.Application.Accounts.Commands.RecreateMachine;
using AccountManager.Application.Accounts.Commands.ResetLastOperation;
using AccountManager.Application.Accounts.Commands.ResetMachineFailureState;
using AccountManager.Application.Accounts.Commands.StartMachine;
using AccountManager.Application.Accounts.Commands.StopMachine;
using AccountManager.Application.Accounts.Commands.TerminateMachine;
using AccountManager.Application.Commands.ChangeInstanceTypeForMachine;
using AccountManager.Application.Commands.ForceBackupForMachine;
using AccountManager.Application.Commands.ForcePopulateForMachine;
using AccountManager.Application.Commands.QueueOperations;
using AccountManager.Application.Commands.RestoreBackupForMachine;
using AccountManager.Application.Queries.GetAllMachines;
using AccountManager.Application.Queries.GetAllOperationsForMachine;
using AccountManager.Application.Queries.GetMachineStats;
using AccountManager.Application.Queries.GetSiteServerStatusForMachine;
using AccountManager.Application.Queries.GetSoftwareInfoForMachine;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/machines")]
    public class MachineApiController : ApiControllerBase
    {
        public MachineApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllMachinesQuery()));
        }

        [HttpGet, Route("stats")]
        public async Task<IHttpActionResult> GetStats()
        {
            return Ok(await Mediator.Send(new GetMachineStatsQuery()));
        }

        [HttpGet, Route("{id}/state-info")]
        public async Task<IHttpActionResult> GetSoftwareInfoForMachine([FromUri] long id)
        {
            var query = new GetSoftwareInfoForMachineQuery() { Id = id };
            return Ok(await Mediator.Send(query));
        }

        [HttpGet, Route("{id}/site-server-status")]
        public async Task<IHttpActionResult> GetSiteServerStatusForMachine(long id)
        {
            return Ok(await Mediator.Send(new GetSiteServerStatusForMachineQuery() { Id = id }));
        }

        [HttpGet, Route("{id}/operations")]
        public async Task<IHttpActionResult> GetOperations([FromUri] long id)
        {
            var query = new GetAllOperationsForMachineQuery() { Id = id };
            return Ok(await Mediator.Send(query));
        }

        #endregion

        #region Commands

        [HttpPost, Route("{id}/start")]
        public async Task<IHttpActionResult> Start([FromUri] long id)
        {
            return Ok(await Mediator.Send(new StartMachineCommand() { Id = id }));
        }

        [HttpPost, Route("{id}/stop")]
        public async Task<IHttpActionResult> Stop([FromUri] long id)
        {
            return Ok(await Mediator.Send(new StopMachineCommand() { Id = id }));
        }

        [HttpPost, Route("{id}/terminate")]
        public async Task<IHttpActionResult> Terminate([FromUri] long id)
        {
            return Ok(await Mediator.Send(new TerminateMachineCommand() { Id = id }));
        }

        [HttpPost, Route("{id}/reset-failure-state")]
        public async Task<IHttpActionResult> ResetFailureState([FromUri] long id)
        {
            return Ok(await Mediator.Send(new ResetMachineFailureStateCommand() { Id = id }));
        }

        [HttpPost, Route("{id}/reset-last-operation")]
        public async Task<IHttpActionResult> ResetLastOperation([FromUri] long id)
        {
            return Ok(await Mediator.Send(new ResetLastOperationCommand() { Id = id }));
        }

        [HttpPost, Route("{id}/recreate")]
        public async Task<IHttpActionResult> Recreate([FromUri] long id)
        {
            return Ok(await Mediator.Send(new RecreateMachineCommand() { MachineId = id }));
        }

        [HttpPost, Route("{id}/force-backup")]
        public async Task<IHttpActionResult> ForceBackup([FromUri] long id, [FromBody] ForceBackupForMachineCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPost, Route("{id}/force-populate")]
        public async Task<IHttpActionResult> ForcePopulate([FromUri] long id, [FromBody] ForcePopulateForMachineCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPost, Route("{id}/restore-backup")]
        public async Task<IHttpActionResult> RestoreBackup([FromUri] long id, [FromBody] RestoreBackupForMachineCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPost, Route("{id}/queue-operations")]
        public async Task<IHttpActionResult> QueueOperations([FromUri] long id, [FromBody] QueueOperationsCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPost, Route("{id}/change-instance-type")]
        public async Task<IHttpActionResult> ChangeInstanceType([FromUri] long id, [FromBody] ChangeInstanceTypeForMachineCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPut, Route("batch")]
        public async Task<IHttpActionResult> BatchUpdate(BatchUpdateMachinesCommand command)
        {
            await Mediator.Send(command);
            return Ok();
        }

        #endregion
    }
}
