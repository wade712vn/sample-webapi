using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Audit.Queries.GetAuditLogs;
using AccountManager.Domain;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/audit")]
    public class AuditApiController : ApiControllerBase
    {
        
        public AuditApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet, Route("logs")]
        public async Task<IHttpActionResult> GetLogs(
            [FromUri] long? accountId = null, 
            [FromUri] string action = null, 
            [FromUri] int startIndex = 0, 
            [FromUri] int limit = 10, 
            [FromUri] SortDirection sortDir = SortDirection.Descending)
        {
            var auditLogs = await Mediator.Send(new GetAuditLogsQuery()
            {
                AccountId = accountId,
                Action = action,
                StartIndex = startIndex,
                Limit = limit,
                SortDirection = sortDir
            });

            return Ok(auditLogs);
        }
    }
}
