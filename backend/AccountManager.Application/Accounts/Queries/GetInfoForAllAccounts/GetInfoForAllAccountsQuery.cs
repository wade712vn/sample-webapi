using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetInfoForAllAccounts
{
    public class GetInfoForAllAccountsQuery : IRequest<IEnumerable<AccountInfoDto>>
    {
    }

    public class GetInfoForAllAccountsQueryHandler : IRequestHandler<GetInfoForAllAccountsQuery, IEnumerable<AccountInfoDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetInfoForAllAccountsQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountInfoDto>> Handle(GetInfoForAllAccountsQuery query, CancellationToken cancellationToken)
        {
            var accounts = await _context.Set<Account>()
                .Include(x => x.Machines)
                .Where(x => !x.IsDeleted && !x.IsTemplate)
                .ToListAsync(cancellationToken);

            return accounts.Select(x => {

                var managed = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value);

                var manualMaintenance = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value 
                    && y.ManualMaintenance.HasValue && y.ManualMaintenance.Value);
                
                var idle = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value
                    && y.Idle.HasValue && y.Idle.Value);

                var stop = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value && y.Stop);

                var needsAdmin = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value && y.NeedsAdmin);
                var turbo = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value && y.Turbo.HasValue && y.Turbo.Value);
                

                return new AccountInfoDto
                {
                    Id = x.Id,
                    MachineCount = x.Machines.Count,
                    NeedsAdmin = needsAdmin,
                    ManualMaintenance = manualMaintenance,
                    Managed = managed,
                    Turbo = turbo,
                    Idle = idle,
                    Stop = stop
                };
                    
            });
        }
    }
}
