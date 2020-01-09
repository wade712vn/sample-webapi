using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Logging;
using AccountManager.Domain.Entities.Audit;
using AccountManager.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Audit
{
    public class AuditLogEventHandler : 
        INotificationHandler<UserActionNotification>,
        INotificationHandler<AccountCreatedEvent>,
        INotificationHandler<InstanceSettingsPushedEvent>
    {
        private readonly ILogger _logger;
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogEventHandler(ILogger logger, IAuditLogRepository auditLogRepository)
        {
            _logger = logger;
            _auditLogRepository = auditLogRepository;
        }

        public async Task Handle(UserActionNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                await _auditLogRepository.AddAsync(new AuditLog()
                {
                    Action = notification.Action,
                    User = notification.User,
                    Time = DateTime.UtcNow,
                    Accounts = Mapper.Map<AccountRef[]>(notification.Accounts),
                    Machines = Mapper.Map<MachineRef[]>(notification.Machines),
                    MetaData = notification.Data
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("CreateAccount", notification.User, DateTime.Now, new[] { Mapper.Map<AccountRef>(notification.Account) }, null, new { Params = notification.Command });
        }

        public async Task Handle(InstanceSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("PushInstanceSettings", notification.Actor, DateTime.Now, new[] { Mapper.Map<AccountRef>(notification.Account) }, null, new { Params = notification.Command, Changes = notification.Changes });
        }

        private async Task AddAuditLogAsync(string action, string user, DateTime time, AccountRef[] accounts, MachineRef[] machines, dynamic metadata)
        {
            try
            {
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Action = action,
                    User = user,
                    Time = time,
                    Accounts = accounts,
                    Machines = machines,
                    MetaData = metadata
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
