using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Logging;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.RecreateMachine;
using AccountManager.Application.Saas.Commands.CreateSite;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines;
using Microsoft.Ajax.Utilities;

namespace AccountManager.Application
{
    public class DesiredStateEventHandler : 
        INotificationHandler<AccountCreatedEvent>,
        INotificationHandler<InstanceSettingsPushedEvent>,
        INotificationHandler<SoftwareUpdatedEvent>,
        INotificationHandler<SiteCreatedEvent>,
        INotificationHandler<MachineRecreatedEvent>
    {
        private readonly ILogger _logger;
        private readonly IAccountManagerDbContext _context;

        public DesiredStateEventHandler(IAccountManagerDbContext context, ILogger logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var desiredState = notification.Account.Machines.First().States.First(x => x.Desired);
                _context.Set<HistoricalDesiredState>().Add(Mapper.Map<HistoricalDesiredState>(desiredState));

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(InstanceSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var desiredStates = notification.Account.Machines.Select(x => x.States.First(y => y.Desired));

                foreach (var desiredState in desiredStates)
                {
                    _context.Set<HistoricalDesiredState>().Add(Mapper.Map<HistoricalDesiredState>(desiredState));
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(SoftwareUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var desiredStates = notification.Accounts.SelectMany(x => x.Machines).Select(x => x.States.First(y => y.Desired))
                    .Union(notification.Machines.Select(x => x.States.First(y => y.Desired)))
                    .DistinctBy(x => x.Id);

                foreach (var desiredState in desiredStates)
                {
                    _context.Set<HistoricalDesiredState>().Add(Mapper.Map<HistoricalDesiredState>(desiredState));
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(SiteCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var desiredState = notification.Site.Machine.States.First(x => x.Desired);
                _context.Set<HistoricalDesiredState>().Add(Mapper.Map<HistoricalDesiredState>(desiredState));

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(MachineRecreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var desiredState = notification.Machine.States.First(x => x.Desired);
                _context.Set<HistoricalDesiredState>().Add(Mapper.Map<HistoricalDesiredState>(desiredState));

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
