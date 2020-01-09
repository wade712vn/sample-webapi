using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Git;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Git.Queries.GetAllRepos
{
    public class GetAllReposQueryHandler : IRequestHandler<GetAllReposQuery, List<RepoDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllReposQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<RepoDto>> Handle(GetAllReposQuery request, CancellationToken cancellationToken)
        {
            var repos = await _context.Set<Repo>().ToListAsync(cancellationToken);
            return await Task.FromResult(Mapper.Map<List<RepoDto>>(repos));
        }
    }
}
