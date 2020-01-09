using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Git;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Common.Extensions;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Git.Queries.GetCommitsForRepo
{
    public class GetCommitsForRepoQueryHandler : IRequestHandler<GetCommitsForRepoQuery, PagedData<CommitDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetCommitsForRepoQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<PagedData<CommitDto>> Handle(GetCommitsForRepoQuery request, CancellationToken cancellationToken)
        {
            var queryable = _context.Set<Commit>()
                .Where(x => x.Repo == request.Repo);

            if (request.BranchId.HasValue)
            {
                queryable = queryable.Where(x => x.Branch != null && x.BranchId == request.BranchId);
            }

            if (request.TagOnly)
            {
                queryable = queryable.Where(x => x.Tag != null && x.Tag != string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                queryable = queryable.Where(x => x.FullHash.Contains(request.Search));
            }

            var total = await queryable.CountAsync(cancellationToken);

            var cloudInstances = await queryable
                .OrderByDescending(x => x.Timestamp)
                .Skip(request.StartIndex)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            return new PagedData<CommitDto> {
                Items = Mapper.Map<IEnumerable<CommitDto>>(cloudInstances),
                TotalItems = total,
                StartIndex = request.StartIndex,
                Limit = request.Limit
            };
        }
    }
}
