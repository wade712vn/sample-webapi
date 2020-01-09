using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Application.License;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateBackupSettings
{
    public class UpdateBackupSettingsCommandHandler : CommandHandlerBase<UpdateBackupSettingsCommand, Unit>
    {
        public UpdateBackupSettingsCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateBackupSettingsCommand command, CancellationToken cancellationToken)
        {
            var account = Context.Set<Account>()
                .Include(x => x.BackupConfig)
                .FirstOrDefault(x => !x.IsDeleted && x.Id == command.AccountId);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);

            var backupConfig = account.BackupConfig;

            if (backupConfig != null)
            {
                Mapper.Map(command, backupConfig);
            }
            else
            {
                account.BackupConfig = Mapper.Map<BackupConfig>(command);
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            await Mediator.Publish(new UserActionNotification()
            {
                User = command.User,
                Accounts = new[] { account },
                Action = "PushBackupSettings",
                Data = new
                {
                    Params = command,
                    Changes = changes
                }

            }, cancellationToken);

            return Unit.Value;
        }
    }
}
