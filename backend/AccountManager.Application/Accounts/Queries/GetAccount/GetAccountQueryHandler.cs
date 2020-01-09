using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Library;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAccount
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAccountQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Class)
                .Include(x => x.Billing)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Include(x => x.BackupConfig)
                .Include(x => x.IdleSchedules)
                .Include(x => x.Sites)
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.Id);

            var accountDto = Mapper.Map<AccountDto>(account);

            var machineConfigDto = accountDto.MachineConfig;
            var libraryFileIds = machineConfigDto.MainLibraryFileIds;

            var libraryFiles = await _context.Set<File>().Where(x => x.Id != 0 && libraryFileIds.Contains(x.Id)).ToListAsync(cancellationToken);
            machineConfigDto.MainLibraryFiles = Mapper.Map<FileDto[]>(libraryFiles.ToArray());

            if (machineConfigDto.MainLibraryFileId.HasValue && machineConfigDto.MainLibraryFileId != 0)
            {
                var mainLibraryFile = await _context.Set<File>()
                    .FirstOrDefaultAsync(x => x.Id == machineConfigDto.MainLibraryFileId, cancellationToken);
                machineConfigDto.MainLibraryFile = Mapper.Map<FileDto>(mainLibraryFile);
            }

            if (machineConfigDto.AccountLibraryFileId.HasValue && machineConfigDto.AccountLibraryFileId != 0)
            {
                var accountLibraryFile = await _context.Set<File>()
                    .FirstOrDefaultAsync(x => x.Id == machineConfigDto.AccountLibraryFileId, cancellationToken);
                machineConfigDto.AccountLibraryFile = Mapper.Map<FileDto>(accountLibraryFile);
            }

            var instanceSettingsTemplates = await _context.Set<MachineConfig>().Where(x => x.IsTemplate).OrderByDescending(x => x.Id).ToListAsync(cancellationToken);
            var matched = instanceSettingsTemplates.FirstOrDefault(x => MatchTemplate(x, account.MachineConfig));
            machineConfigDto.BundleVersion = matched == null ? "Custom" : matched.Name;

            return await Task.FromResult(accountDto);
        }

        private static bool MatchTemplate(MachineConfig templateConfig, MachineConfig machineConfig)
        {
            return new VersionInfo(machineConfig.LauncherHash).Hash == new VersionInfo(templateConfig.LauncherHash).Hash &&
                   new VersionInfo(machineConfig.ReportingHash).Hash == new VersionInfo(templateConfig.ReportingHash).Hash &&
                   new VersionInfo(machineConfig.PdfExportHash).Hash == new VersionInfo(templateConfig.PdfExportHash).Hash &&
                   new VersionInfo(machineConfig.SiteMasterHash).Hash == new VersionInfo(templateConfig.SiteMasterHash).Hash &&
                   new VersionInfo(machineConfig.ClientHash).Hash == new VersionInfo(templateConfig.ClientHash).Hash &&
                   new VersionInfo(machineConfig.RelExportHash).Hash == new VersionInfo(templateConfig.RelExportHash).Hash &&
                   new VersionInfo(machineConfig.DeployerHash).Hash == new VersionInfo(templateConfig.DeployerHash).Hash &&
                   new VersionInfo(machineConfig.PopulateHash).Hash == new VersionInfo(templateConfig.PopulateHash).Hash;
        }
    }


}
