using System;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateGeneralTemplate
{
    public class CreateOrUpdateGeneralTemplateCommandHandler : IRequestHandler<CreateOrUpdateGeneralTemplateCommand, long>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateGeneralTemplateCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateOrUpdateGeneralTemplateCommand command, CancellationToken cancellationToken)
        {
            var transaction = _context.GetTransaction();
            try
            {
                Account accountTemplate;
                if (command.Id > 0)
                {
                    accountTemplate = await UpdateTemplate(command, cancellationToken);
                }
                else
                {
                    accountTemplate = await CreateTemplate(command, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                transaction.Commit();
                return accountTemplate.Id;
            }
            catch (Exception e)
            {
                 transaction.Rollback();
                throw;
            }
        }

        private async Task<Account> CreateTemplate(CreateOrUpdateGeneralTemplateCommand command, CancellationToken cancellationToken)
        {
            var accountTemplate = new Account() { IsTemplate = true };
            Mapper.Map(command, accountTemplate);

            var instanceSettingsTemplate = new MachineConfig() { IsTemplate = true };
            Mapper.Map(command, instanceSettingsTemplate);
            accountTemplate.MachineConfig = instanceSettingsTemplate;

            var licenseTemplate = new LicenseConfig() { IsTemplate = true };
            Mapper.Map(command, licenseTemplate);
            accountTemplate.LicenseConfig = licenseTemplate;

            var backupSettingsTemplate = new BackupConfig() { IsTemplate = true };
            Mapper.Map(command, backupSettingsTemplate);
            accountTemplate.BackupConfig = backupSettingsTemplate;

            _context.Set<Account>().Add(accountTemplate);
            return accountTemplate;
        }

        private async Task<Account> UpdateTemplate(CreateOrUpdateGeneralTemplateCommand command, CancellationToken cancellationToken)
        {
            var accountTemplate = await _context.Set<Account>()
                .Include(x => x.Billing)
                .Include(x => x.Contact)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Include(x => x.BackupConfig)
                .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == command.Id, cancellationToken);

            if (accountTemplate == null)
                throw new EntityNotFoundException(nameof(Account), command.Id);

            var instanceSettingsTemplate = accountTemplate.MachineConfig;
            if (instanceSettingsTemplate != null)
            {
                Mapper.Map(command, instanceSettingsTemplate);
            }
            else
            {
                instanceSettingsTemplate = new MachineConfig() { IsTemplate = true };
                Mapper.Map(command, instanceSettingsTemplate);
                accountTemplate.MachineConfig = instanceSettingsTemplate;
            }

            var licenseTemplate = accountTemplate.LicenseConfig;
            if (licenseTemplate != null)
            {
                Mapper.Map(command, licenseTemplate);
            }
            else
            {
                licenseTemplate = new LicenseConfig() { IsTemplate = true };
                Mapper.Map(command, licenseTemplate);
                accountTemplate.LicenseConfig = licenseTemplate;
            }

            var backupSettingsTemplate = accountTemplate.BackupConfig;
            if (backupSettingsTemplate != null)
            {
                Mapper.Map(command, backupSettingsTemplate);
            }
            else
            {
                backupSettingsTemplate = new BackupConfig() { IsTemplate = true };
                Mapper.Map(command, backupSettingsTemplate);
                accountTemplate.BackupConfig = backupSettingsTemplate;
            }

            Mapper.Map(command, accountTemplate);

            return accountTemplate;
        }
    }
}