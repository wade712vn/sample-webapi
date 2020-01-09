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

namespace AccountManager.Application.Accounts.Commands.UpdateIdleSchedule
{
    public class UpdateIdleScheduleCommandValidator : AbstractValidator<UpdateIdleScheduleCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public UpdateIdleScheduleCommandValidator(IAccountManagerDbContext context)
        {
            _context = context;

            RuleFor(x => x).CustomAsync(IdleScheduleNotConflictWithBackupSettings);
        }

        private async Task IdleScheduleNotConflictWithBackupSettings(UpdateIdleScheduleCommand command, CustomContext context, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.BackupConfig)
                .FirstOrDefaultAsync(x => x.Id == command.AccountId, cancellationToken);

            if (account == null || account.IsDeleted)
            {
                context.AddFailure("Accounts not found");
                return;
            }

            var idleSchedules = command.IdleSchedules;
            var backupTimes = DeserializeBackupTimes(account.BackupConfig.Times);

            foreach (var backupTime in backupTimes)
            {
                foreach (var idleSchedule in idleSchedules)
                {
                    var from = idleSchedule.StopAt;
                    var to = from.AddHours(idleSchedule.ResumeAfter);
                    if (backupTime <= to && backupTime >= from)
                    {
                        context.AddFailure(new ValidationFailure("idleSchedules", "Idle schedule has conflict with backup settings", idleSchedule));
                    }
                }
            }
        }

        protected DateTimeOffset[] DeserializeBackupTimes(string sTimes)
        {
            try
            {
                sTimes = sTimes.Replace("{", "[").Replace("}", "]");
                return JsonConvert.DeserializeObject<DateTimeOffset[]>(sTimes).ToArray();
            }
            catch (Exception e)
            {
                //
                return new DateTimeOffset[] { };
            }
        }
    }
}
