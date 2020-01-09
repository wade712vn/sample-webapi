using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Services;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;
using Machine = AccountManager.Domain.Entities.Machine;

namespace AccountManager.Application.Saas.Queries.GetSiteStatus
{
    public class GetSiteStatusQuery : IRequest<SiteStatusDto>
    {
        public string AccountUrlFriendlyName { get; set; }
        public string UrlFriendlyName { get; set; }
    }

    public class GetSiteStatusQueryHandler : IRequestHandler<GetSiteStatusQuery, SiteStatusDto>
    {
        private readonly IAccountManagerDbContext _context;
        private readonly ITaskTrackingService _taskTrackingService;

        public GetSiteStatusQueryHandler(IAccountManagerDbContext context, ITaskTrackingService taskTrackingService)
        {
            _context = context;
            _taskTrackingService = taskTrackingService;
        }

        public async Task<SiteStatusDto> Handle(GetSiteStatusQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UrlFriendlyName == request.AccountUrlFriendlyName, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountUrlFriendlyName);

            var site = await _context.Set<Site>()
                .Include(x => x.Machine)
                .FirstOrDefaultAsync(x => x.AccountId == account.Id && x.UrlFriendlyName == request.UrlFriendlyName, cancellationToken);

            if (site == null)
                throw new EntityNotFoundException(nameof(Site), request.UrlFriendlyName);

            var machine = site.Machine;

            if (machine == null)
                return null;

            var desiredState = await _context.Set<State>()
                .FirstOrDefaultAsync(x => x.MachineId == machine.Id && x.Desired, cancellationToken);

            var instance = await _context.Set<CloudInstance>()
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(x => x.MachineId == machine.Id && x.Active, cancellationToken);

            var activeOperation = await _context.Set<Operation>()
                .Include(x => x.Type)
                .FirstOrDefaultAsync(x => x.MachineId == machine.Id && x.Active, cancellationToken);

            var activeTasks = _taskTrackingService.GetActiveTasksForSite(site.Id);

            var currentTask = activeTasks.FirstOrDefault();

            var siteStatus = new SiteStatusDto()
            {
                ActiveTask = currentTask,
                InstanceStatus = instance?.Status,
                SiteStatus = GetSiteStatus(machine, instance, currentTask, activeOperation)
            };
            
            return await Task.FromResult(siteStatus);
        }

        private static string GetSiteStatus(Machine machine, CloudInstance instance, SaasTaskBase currentTask, Operation activeOperation)
        {
            if (!machine.NeedsAdmin && instance?.Status == "running" && currentTask == null)
            {
                return "ready";
            }

            if (machine.NeedsAdmin)
            {
                return "error";
            }

            if (currentTask != null)
            {
                return currentTask.StatusDescription;
            }

            if (activeOperation != null)
            {
                return $"{activeOperation.Type.Description} in progress";
            }

            return "unknown";
        }
    }
}
