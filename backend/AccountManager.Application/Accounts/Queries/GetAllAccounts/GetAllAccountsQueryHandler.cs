using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, List<AccountListingDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllAccountsQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccountListingDto>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Class)
                .Include(x => x.Billing)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Where(x => !x.IsDeleted && !x.IsTemplate)
                .ToListAsync(cancellationToken);

            var instanceSettingsTemplates = await _context.Set<MachineConfig>().Where(x => x.IsTemplate).OrderByDescending(x => x.Id).ToListAsync(cancellationToken);

            var accountDtos = new List<AccountListingDto>();
            foreach (var account in accounts)
            {
                var matched = instanceSettingsTemplates.FirstOrDefault(x => MatchTemplate(x, account.MachineConfig));
                var accountDto = Mapper.Map<AccountListingDto>(account);
                accountDto.BundleVersion = matched == null ? "Custom" : matched.Name;
                accountDtos.Add(accountDto);
            }

            return await Task.FromResult(accountDtos);
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
