using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AccountManager.Application.Git.Queries.GetAllBranches;
using AccountManager.Application.Git.Queries.GetAllRepos;
using AccountManager.Application.Git.Queries.GetCommitsForRepo;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/" + ApiVersion + "/git")]
    public class GitApiController : ApiControllerBase
    {
        public GitApiController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet, Route("repos")]
        public async Task<IHttpActionResult> GetAllRepos()
        {
            return Ok(await Mediator.Send(new GetAllReposQuery()));
        }

        [HttpGet, Route("repos/{name}/commits")]
        public async Task<IHttpActionResult> GetCommitsForRepo(string name, [FromUri] int? branch = null, [FromUri] bool tagOnly = false, [FromUri] string search = null, [FromUri] int startIndex = 0, [FromUri] int limit = 20)
        {
            return Ok(await Mediator.Send(new GetCommitsForRepoQuery()
            {
                Repo = name,
                BranchId = branch,
                TagOnly = tagOnly,
                Search = search,
                StartIndex = startIndex,
                Limit = limit
            }));
        }

        [HttpGet, Route("branches")]
        public async Task<IHttpActionResult> GetAllBranches()
        {
            return Ok(await Mediator.Send(new GetAllBranchesQuery()));
        }
    }
}
