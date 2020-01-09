using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Accounts.Queries.GetAllMachinesForAccount;
using AccountManager.Application.Notification.Commands.SendMessage;
using AccountManager.Application.Saas.Commands.CreateSite;
using AccountManager.Application.Saas.Commands.DeleteSite;
using AccountManager.Application.Saas.Queries.GetSiteStatus;
using MediatR;

namespace AccountManager.WebApi.Controllers.SaasApi
{
    [RoutePrefix("saas/api/" + ApiVersion + "/accounts")]
    public class SaasAccountApiController : SaasApiControllerBase
    {
        public SaasAccountApiController(IMediator mediator) : base(mediator)
        {

        }

        [HttpGet, Route("{urlFriendlyName}/machines")]
        public async Task<IHttpActionResult> GetMachines([FromUri] string urlFriendlyName)
        {
            var query = new GetAllMachinesForAccountQuery()
            {
                UrlFriendlyName = urlFriendlyName
            };

            return Ok(await Mediator.Send(query));
        }

        [HttpPost, Route("{urlFriendlyName}/sites")]
        public async Task<IHttpActionResult> CreateSite([FromUri] string urlFriendlyName, [FromBody] CreateSiteCommand command)
        {
            command.AccountUrlFriendlyName = urlFriendlyName;
            return Ok(await Mediator.Send(command));
        }

        [HttpGet, Route("{urlFriendlyName}/sites/{siteUrlFriendlyName}/status")]
        public async Task<IHttpActionResult> GetSiteStatus([FromUri] string urlFriendlyName, [FromUri] string siteUrlFriendlyName)
        {
            var query = new GetSiteStatusQuery
            {
                AccountUrlFriendlyName = urlFriendlyName,
                UrlFriendlyName = siteUrlFriendlyName
            };

            return Ok(await Mediator.Send(query));
        }

        [HttpDelete, Route("{urlFriendlyName}/sites/{siteUrlFriendlyName}")]
        public async Task<IHttpActionResult> DeleteSite([FromUri] string urlFriendlyName, [FromUri] string siteUrlFriendlyName, [FromBody] DeleteSiteCommand command)
        {
            command.AccountUrlFriendlyName = urlFriendlyName;
            command.UrlFriendlyName = siteUrlFriendlyName;
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Route("{urlFriendlyName}/notifications")]
        public async Task<IHttpActionResult> CreateNotification([FromUri] string urlFriendlyName, [FromBody] SendMessageCommand command)
        {
            command.Accounts = null;
            command.AccountUrlNames = new[] {urlFriendlyName};
            return Ok(await Mediator.Send(command));
        }

        
    }
}
