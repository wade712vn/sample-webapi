using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Templates.Queries.GetAllInstanceSettingsTemplates;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllBackupSettingsTemplates
{
    public class GetAllBackupSettingsTemplatesQueryHandler : IRequestHandler<GetAllBackupSettingsTemplatesQuery, IEnumerable<BackupConfigDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllBackupSettingsTemplatesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BackupConfigDto>> Handle(GetAllBackupSettingsTemplatesQuery request, CancellationToken cancellationToken)
        {
            var backupSettingsTemplates = await _context.Set<BackupConfig>()
                .Include(x => x.Account)
                .Where(x => x.Account == null || !x.Account.IsDeleted)
                .ToListAsync(cancellationToken);

            return Mapper.Map<List<BackupConfigDto>>(backupSettingsTemplates);
        }
    }
}