using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.UpdateBackupSettings;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Application.Commands;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Logging;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application
{
    public class UserOperationEventHandler :
        INotificationHandler<AccountCreatedEvent>,
        INotificationHandler<MachineActionTriggeredEvent>,
        INotificationHandler<InstanceSettingsPushedEvent>,
        INotificationHandler<BackupSettingsPushedEvent>,
        INotificationHandler<LicenseSettingsPushedEvent>

    {
        private readonly ILogger _logger;
        private readonly IAccountManagerDbContext _context;

        public UserOperationEventHandler(ILogger logger, IAccountManagerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
        {
            await AddUserOperation(UserOperationTypes.CreateAccount, notification.Account.Machines.First().Id, notification.User, cancellationToken);
        }

        public async Task Handle(InstanceSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var machine in notification.Account.Machines)
            {
                await AddUserOperation(UserOperationTypes.PushInstanceSettings, machine.Id, notification.Actor, cancellationToken);
            }
        }

        public async Task Handle(BackupSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
        }

        public async Task Handle(LicenseSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
        }

        public async Task Handle(MachineActionTriggeredEvent notification, CancellationToken cancellationToken)
        {
            await AddUserOperation(notification.Action, notification.Machine.Id, notification.User, cancellationToken);
        }

        private async Task AddUserOperation(string type, long machineId, string user, CancellationToken cancellationToken)
        {
            _context.Set<UserOperation>().Add(new UserOperation
            {
                TypeName = type,
                MachineId = machineId,
                Timestamp = DateTimeOffset.UtcNow,
                User = user,
            });
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
