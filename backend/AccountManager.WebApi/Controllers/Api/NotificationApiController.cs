using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Notification.Commands.SendMessage;
using AccountManager.Application.Notification.Queries.ListPendingMessage;
using AutoMapper;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/notification")]
    public class NotificationApiController : ApiControllerBase
    {
        public NotificationApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet, Route("messages")]
        public async Task<IHttpActionResult> ListPendingMessages()
        {
            var messages = await Mediator.Send(new ListPendingMessagesQuery());

            return Ok(Mapper.Map<IEnumerable<MessageDto>>(messages));
        }

        [HttpPost, Route("send")]
        public async Task<IHttpActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            await Mediator.Send(command);

            return Ok();
        }
    }
}
