using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateGeneralTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateLicenseTemplate;
using AccountManager.Application.Templates.Commands.DeleteTemplate;
using AccountManager.Application.Templates.Queries.GetAllBackupSettingsTemplates;
using AccountManager.Application.Templates.Queries.GetAllGeneralTemplates;
using AccountManager.Application.Templates.Queries.GetAllInstanceSettingsTemplates;
using AccountManager.Application.Templates.Queries.GetAllLicenseTemplates;
using AccountManager.WebApi.Filters;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [JwtAuthenticationFilter]
    [RoutePrefix("api/" + ApiVersion + "/template")]
    public class TemplateApiController : ApiControllerBase
    {
        public TemplateApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet, Route("license-settings")]
        public async Task<IHttpActionResult> GetAllLicenseTemplates()
        {
            return Ok(await Mediator.Send(new GetAllLicenseTemplatesQuery()));
        }

        [HttpGet, Route("instance-settings")]
        public async Task<IHttpActionResult> GetAllInstanceSettingsTemplates()
        {
            return Ok(await Mediator.Send(new GetAllInstanceSettingsTemplatesQuery()));
        }

        [HttpGet, Route("backup-settings")]
        public async Task<IHttpActionResult> GetAllBackupSettingsTemplates()
        {
            return Ok(await Mediator.Send(new GetAllBackupSettingsTemplatesQuery()));
        }

        [HttpGet, Route("general")]
        public async Task<IHttpActionResult> GetAllGeneralTemplates()
        {
            return Ok(await Mediator.Send(new GetAllGeneralTemplatesQuery()));
        }

        #endregion

        #region Commands

        [HttpPost, Route("license-settings")]
        public async Task<IHttpActionResult> CreateLicenseTemplate([FromBody] CreateOrUpdateLicenseTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Route("instance-settings")]
        public async Task<IHttpActionResult> CreateInstanceSettingsTemplate([FromBody] CreateOrUpdateInstanceSettingsTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Route("backup-settings")]
        public async Task<IHttpActionResult> CreateBackupSettingsTemplate([FromBody] CreateOrUpdateBackupSettingsTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Route("general")]
        public async Task<IHttpActionResult> CreateGeneralTemplate([FromBody] CreateOrUpdateGeneralTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("license-settings/{id}")]
        public async Task<IHttpActionResult> UpdateLicenseTemplate(long id, [FromBody] CreateOrUpdateLicenseTemplateCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("instance-settings/{id}")]
        public async Task<IHttpActionResult> UpdateInstanceSettingsTemplate(long id, [FromBody] CreateOrUpdateInstanceSettingsTemplateCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("backup-settings/{id}")]
        public async Task<IHttpActionResult> UpdateBackupSettingsTemplate(long id, [FromBody] CreateOrUpdateBackupSettingsTemplateCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpPut, Route("general/{id}")]
        public async Task<IHttpActionResult> UpdateGeneralTemplate(long id, [FromBody] CreateOrUpdateGeneralTemplateCommand command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete, Route("{type}/{id}")]
        public async Task<IHttpActionResult> DeleteTemplate([FromUri] string type, [FromUri] long id)
        {
            return Ok(await Mediator.Send(new DeleteTemplateCommand
            {
                Id = id,
                Type = type
            }));
        }

        #endregion
    }
}
