using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AccountManager.Application.Accounts.Commands.ActivateAccount;
using AccountManager.Application.Accounts.Commands.BatchUpdateAccounts;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.CreateMachine;
using AccountManager.Application.Accounts.Commands.DeactivateAccount;
using AccountManager.Application.Accounts.Commands.DeleteAccount;
using AccountManager.Application.Accounts.Commands.SendCreationMail;
using AccountManager.Application.Accounts.Commands.UpdateAccount;
using AccountManager.Application.Accounts.Commands.UpdateBackupSettings;
using AccountManager.Application.Accounts.Commands.UpdateIdleSchedule;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Application.Accounts.Commands.UpdateMachine;
using AccountManager.Application.Accounts.Queries.DownloadAllKeys;
using AccountManager.Application.Accounts.Queries.DownloadLicenseFile;
using AccountManager.Application.Accounts.Queries.DownloadLicenseKeys;
using AccountManager.Application.Accounts.Queries.GetAccount;
using AccountManager.Application.Accounts.Queries.GetAllAccounts;
using AccountManager.Application.Accounts.Queries.GetAllMachinesForAccount;
using AccountManager.Application.Accounts.Queries.GetAllSitesForAccount;
using AccountManager.Application.Accounts.Queries.GetCreatableMachinesForAccount;
using AccountManager.Application.Accounts.Queries.GetInfoForAllAccounts;
using AccountManager.Application.Accounts.Queries.GetInstanceSettingsForAccount;
using AccountManager.Application.Queries.GetAllOperationsForMachine;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/accounts")]
    public class AccountApiController : ApiControllerBase
    {

        public AccountApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllAccountsQuery()));
        }

        [HttpGet, Route("info")]
        public async Task<IHttpActionResult> GetInfoForAll()
        {
            return Ok(await Mediator.Send(new GetInfoForAllAccountsQuery()));
        }

        [HttpGet, Route("{id}")]
        public async Task<IHttpActionResult> Get(long id)
        {
            return Ok(await Mediator.Send(new GetAccountQuery() { Id = id }));
        }

        [HttpGet, Route("{id}/instance-settings")]
        public async Task<IHttpActionResult> GetInstanceSettingsForAccount(long id)
        {
            return Ok(await Mediator.Send(new GetInstanceSettingsForAccountQuery() { Id = id }));
        }

        [HttpGet, Route("{id}/sites")]
        public async Task<IHttpActionResult> GetAllSitesForAccount(long id)
        {
            return Ok(await Mediator.Send(new GetAllSitesForAccountQuery() { Id = id }));
        }

        [HttpGet, Route("{id}/machines")]
        public async Task<IHttpActionResult> GetAllMachinesForAccount(long id)
        {
            return Ok(await Mediator.Send(new GetAllMachinesForAccountQuery() { Id = id }));
        }

        [HttpGet, Route("{id}/creatable-machines")]
        public async Task<IHttpActionResult> GetCreatableMachinesForAccount(long id)
        {
            return Ok(await Mediator.Send(new GetCreatableMachinesForAccountQuery() { Id = id }));
        }

        [HttpGet, Route("{id}/files/license")]
        public async Task<HttpResponseMessage> DownloadLicenseFile(long id)
        {
            var downloadable = await Mediator.Send(new DownloadLicenseFileQuery { AccountId = id });
            var response = CreateFileResponse(downloadable.Content, downloadable.FileName);

            return await Task.FromResult(response);
        }

        [HttpGet, Route("{id}/files/license-keys")]
        public async Task<HttpResponseMessage> DownloadLicenseKeys(long id)
        {
            var downloadable = await Mediator.Send(new DownloadLicenseKeysQuery { AccountId = id });
            var response = CreateFileResponse(downloadable.Content, downloadable.FileName);

            return await Task.FromResult(response);
        }

        [HttpGet, Route("{id}/files/keys")]
        public async Task<HttpResponseMessage> DownloadAllKeys(long id)
        {
            var downloadable = await Mediator.Send(new DownloadAllKeysQuery { AccountId = id });
            var response = CreateFileResponse(downloadable.Content, downloadable.FileName);

            return await Task.FromResult(response);
        }

        private HttpResponseMessage CreateFileResponse(byte[] content, string fileName)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            var stream = new MemoryStream(content);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = fileName };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return response;
        }

        #endregion

        #region Commands

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateAccountCommand command)
        {
            var accountId = await Mediator.Send(command);
            return Ok(accountId);
        }

        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] long id, UpdateAccountCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPost, Route("{id}/deactivate")]
        public async Task<IHttpActionResult> Deactivate(long id)
        {
            var command = new DeactivateAccountCommand { Id = id };
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Route("{id}/activate")]
        public async Task<IHttpActionResult> Activate(long id)
        {
            var command = new ActivateAccountCommand { Id = id, StartMachines = true };
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Route("{id}/send-creation-mail")]
        public async Task<IHttpActionResult> SendCreationMail(long id)
        {
            var command = new SendCreationMailCommand { AccountId = id};
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> Delete(long id)
        {
            await Mediator.Send(new DeleteAccountCommand() { Id = id });
            return Ok();
        }

        [HttpPatch, Route("{id}/machines/{machineId}")]
        public async Task<IHttpActionResult> Update([FromUri] long id, [FromUri] long machineId, [FromBody] UpdateMachineCommand command)
        {
            command.AccountId = id;
            command.MachineId = machineId;

            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("{id}/license-settings")]
        public async Task<IHttpActionResult> UpdateLicenseSettings([FromUri] long id, [FromBody] UpdateLicenseSettingsCommand settingsCommand)
        {
            settingsCommand.AccountId = id;
            return Ok(await Mediator.Send(settingsCommand));
        }

        [HttpPut, Route("{id}/instance-settings")]
        public async Task<IHttpActionResult> UpdateInstanceSettings([FromUri] long id, [FromBody] UpdateInstanceSettingsCommand command)
        {
            command.AccountId = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("{id}/backup-settings")]
        public async Task<IHttpActionResult> UpdateBackupSettings([FromUri] long id, [FromBody] UpdateBackupSettingsCommand command)
        {
            command.AccountId = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("{id}/idle-schedule")]
        public async Task<IHttpActionResult> UpdateIdleSchedule([FromUri] long id, [FromBody] UpdateIdleScheduleCommand command)
        {
            command.AccountId = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("batch")]
        public async Task<IHttpActionResult> BatchUpdate(BatchUpdateAccountsCommand command)
        {
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPost, Route("{id}/machines")]
        public async Task<IHttpActionResult> CreateMachineForAccount([FromUri] long id, [FromBody] CreateMachineCommand command)
        {
            command.AccountId = id;
            await Mediator.Send(command);
            return Ok();
        }

        #endregion
    }
}
