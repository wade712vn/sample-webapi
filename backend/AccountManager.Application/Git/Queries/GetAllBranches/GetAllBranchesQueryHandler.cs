using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Git;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Git.Queries.GetAllBranches
{
    public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllBranchesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
        {
            var branches = await _context.Set<Branch>().ToListAsync(cancellationToken);
            return await Task.FromResult(Mapper.Map<List<BranchDto>>(branches));
        }
    }
}
