using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Commands.UpdateBackupSettings
{
    public class UpdateBackupSettingsCommandValidator : AbstractValidator<UpdateBackupSettingsCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public UpdateBackupSettingsCommandValidator(IAccountManagerDbContext context)
        {
            _context = context;

            RuleFor(x => x).CustomAsync(BackupSettingsNotConflictWithIdleSchedule);
        }

        private async Task BackupSettingsNotConflictWithIdleSchedule(UpdateBackupSettingsCommand command, CustomContext context, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.IdleSchedules)
                .FirstOrDefaultAsync(x => x.Id == command.AccountId, cancellationToken);

            if (account == null || account.IsDeleted)
            {
                context.AddFailure("Accounts not found");
                return;
            }

            var idleSchedules = account.IdleSchedules;
            var backupTimes = command.Times;

            foreach (var idleSchedule in idleSchedules)
            {
                var from = idleSchedule.StopAt;
                var to = from.AddHours(idleSchedule.ResumeAfter);

                foreach (var backupTime in backupTimes)
                {
                    if (backupTime <= to && backupTime >= from)
                    {
                        context.AddFailure(new ValidationFailure("backupTimes", "Backup time has conflict with idle schedule", backupTime));
                    }
                }
            }
        }

        
    }
}
