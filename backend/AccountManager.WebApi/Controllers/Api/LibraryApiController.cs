using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Git.Queries.GetAllRepos;
using AccountManager.Application.Library.Commands.BatchDeleteLibraryFiles;
using AccountManager.Application.Library.Commands.DeleteLibraryFile;
using AccountManager.Application.Library.Queries.GetAllLibraryFiles;
using AccountManager.Application.Library.Queries.GetAllLibraryPackages;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/library")]
    public class LibraryApiController : ApiControllerBase
    {
        public LibraryApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet, Route("files")]
        public async Task<IHttpActionResult> GetAllFiles()
        {
            return Ok(await Mediator.Send(new GetAllLibraryFilesQuery()));
        }

        [HttpGet, Route("packages")]
        public async Task<IHttpActionResult> GetAllPackages()
        {
            return Ok(await Mediator.Send(new GetAllLibraryPackagesQuery()));
        }

        #region Commands

        [HttpDelete, Route("files/{id}")]
        public async Task<IHttpActionResult> DeleteFile([FromUri] long id)
        {
            return Ok(await Mediator.Send(new DeleteLibraryFileCommand() { Id = id }));
        }

        [HttpDelete, Route("files/batch")]
        public async Task<IHttpActionResult> BatchDeleteFiles([FromUri] string ids)
        {
            var fileIds = ids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

            return Ok(await Mediator.Send(new BatchDeleteLibraryFilesCommand() { Ids = fileIds }));
        }

        #endregion
    }
}
