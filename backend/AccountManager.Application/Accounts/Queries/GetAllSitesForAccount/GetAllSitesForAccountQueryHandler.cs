using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Queries.GetAccount;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllSitesForAccount
{
    public class GetAllSitesForAccountQueryHandler : IRequestHandler<GetAllSitesForAccountQuery, IEnumerable<SiteDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllSitesForAccountQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SiteDto>> Handle(GetAllSitesForAccountQuery request, CancellationToken cancellationToken)
        {
            var sites = await _context.Set<Site>()
                .Where(x => x.AccountId == request.Id)
                .ToListAsync(cancellationToken);
            
            return await Task.FromResult(Mapper.Map<IEnumerable<SiteDto>>(sites));
        }
    }
}
