using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Services;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using MediatR;
using Newtonsoft.Json;

namespace AccountManager.Application.Saas.Commands.CreateSite
{
    public class CreateSiteCommandHandler : CommandHandlerBase<CreateSiteCommand, Unit>
    {
        private readonly ITaskTrackingService _taskTrackingService;

        public CreateSiteCommandHandler(IMediator mediator, IAccountManagerDbContext context, ITaskTrackingService taskTrackingService) : base(mediator, context)
        {
            _taskTrackingService = taskTrackingService;
        }

        public override async Task<Unit> Handle(CreateSiteCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)

                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UrlFriendlyName == command.AccountUrlFriendlyName, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountUrlFriendlyName);

            var machineConfig = account.MachineConfig;

            var site = new Site()
            {
                Name = command.Name,
                UrlFriendlyName = command.UrlFriendlyName,
                CloudInstanceType = account.LicenseConfig.CloudInstanceType,
                Account = account,
                AccountId = account.Id
            };

            Context.Set<Site>().Add(site);

            var siteMasterVersion = new VersionInfo(machineConfig.SiteMasterHash);
            var clientVersion = new VersionInfo(machineConfig.ClientHash);
            var relExportVersion = new VersionInfo(machineConfig.RelExportHash);
            var deployerVersion = new VersionInfo(machineConfig.DeployerHash);
            var populateVersion = new VersionInfo(machineConfig.PopulateHash);
            var linkwareVersion = new VersionInfo(machineConfig.LinkwareHash);

            var state = new State()
            {
                Launcher = Versions.None,
                SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Hash,
                Client = clientVersion.IsNone() ? Versions.None : clientVersion.Hash,
                RelExport = relExportVersion.IsNone() ? Versions.None : relExportVersion.Hash,
                Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Hash,
                PdfExport = Versions.None,
                Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Hash,
                Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Hash,
                Reporting = Versions.None,
                LibraryFile = machineConfig.MainLibraryFile ?? 0,
                LibraryFiles = machineConfig.MainLibraryFiles,
                AccountLibraryFile = machineConfig.AccountLibraryFile ?? 0,

                Locked = false,
                Desired = true,
                SslEnabled = machineConfig.EnableSsl,
                
            };

            var cloudInstanceType = await Context.Set<CloudInstanceType>()
                .FirstOrDefaultAsync(x => x.CloudCode == command.CloudInstanceType, cancellationToken);

            var baseDomainUrl = "";
            var machine = new Machine()
            {
                IsLauncher = false,
                IsSiteMaster = true,
                CreationMailSent = false,
                CloudInstanceTypeId = cloudInstanceType?.Id ?? account.LicenseConfig.CloudInstanceType,
                MailTo = account.Contact.Email,
                RdpUsers = JsonConvert.SerializeObject(new { }),
                States = new List<State>() { state },
                ClassId = account.ClassId,
                Managed = account.Managed,
                Turbo = true, 
                Region = machineConfig.Region,
                VmImageName = machineConfig.VmImageName,
                Account = account,
                AccountId = account.Id,
                SampleData = machineConfig.IncludeSampleData,
                SiteName = site.UrlFriendlyName,
                SiteMasterUrl = $"{(machineConfig.EnableSsl ? "https" : "http")}://{site.UrlFriendlyName}.{account.UrlFriendlyName}.{baseDomainUrl}",
                Name = $"MMA {account.UrlFriendlyName} IPSM SiteMaster {command.UrlFriendlyName}"
            };

            Context.Set<Machine>().Add(machine);

            site.Machine = machine;

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new SiteCreatedEvent()
            {
                Actor = command.User,
                Site = site,
                Command = command

            }, cancellationToken);

            var task = new CreateSiteTask
            {
                AccountId = account.Id,
                SiteId = site.Id,
                MachineId = machine.Id
            };
            _taskTrackingService.Track(task);

            return await Task.FromResult(Unit.Value);
        }
    }
}