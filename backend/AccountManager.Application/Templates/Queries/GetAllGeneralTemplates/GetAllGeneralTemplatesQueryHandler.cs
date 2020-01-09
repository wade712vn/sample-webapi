using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllGeneralTemplates
{
    public class GetAllGeneralTemplatesQueryHandler : IRequestHandler<GetAllGeneralTemplatesQuery, IEnumerable<AccountDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllGeneralTemplatesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountDto>> Handle(GetAllGeneralTemplatesQuery request, CancellationToken cancellationToken)
        {
            var generalTemplates = await _context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Billing)
                .Include(x => x.BackupConfig)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return Mapper.Map<List<AccountDto>>(generalTemplates);
        }
    }
}