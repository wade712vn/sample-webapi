using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Utils;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Commands.CreateMachine
{
    public class CreateMachineCommand : CommandBase
    {
        public long AccountId { get; set; }
        public long? SiteId { get; set; }
        public bool IsLauncher { get; set; }
    }

    public class CreateMachineCommandHandler : CommandHandlerBase<CreateMachineCommand, Unit>
    {
        public CreateMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(CreateMachineCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Include(x => x.Sites)
                .FirstOrDefaultAsync(x => !x.IsDeleted && !x.IsTemplate && x.Id == command.AccountId, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);
            
            var machines = await Context.Set<Machine>().Where(x => x.AccountId == account.Id)
                .ToListAsync(cancellationToken);

            if (command.IsLauncher)
            {
                var launcherMachine = machines.FirstOrDefault(x => x.IsLauncher);
                if (launcherMachine != null)
                    throw new CommandException();
            }

            Site site = null;
            if (command.SiteId.HasValue)
            {
                site = account.Sites.FirstOrDefault(x => x.Id == command.SiteId);
                if (site == null)
                    throw new CommandException();

                var siteMachine = machines.FirstOrDefault(x => x.SiteName == site.UrlFriendlyName);
                if (siteMachine != null)
                    throw new CommandException();
            }

            var machineConfig = account.MachineConfig;
            var newState = new State()
            {
                Launcher = command.IsLauncher ? machineConfig.LauncherHash : Versions.None,
                PdfExport = command.IsLauncher ? machineConfig.PdfExportHash : Versions.None,
                Reporting = command.IsLauncher ? machineConfig.ReportingHash : Versions.None,

                Client = machineConfig.ClientHash,
                SiteMaster = machineConfig.SiteMasterHash,
                RelExport = machineConfig.RelExportHash,

                Deployer = machineConfig.DeployerHash,
                Populate = machineConfig.PopulateHash,
                LibraryFile = machineConfig.MainLibraryFile ?? 0,
                LibraryFiles = machineConfig.MainLibraryFiles,
                AccountLibraryFile = machineConfig.AccountLibraryFile ?? 0,

                Locked = false,
                Desired = true,
                SslEnabled = machineConfig.EnableSsl,
            };

            var machine = new Machine()
            {
                Name = AccountUtils.GenerateMachineName(account.UrlFriendlyName, account.LicenseConfig.InstancePolicy),
                Account = account,
                CloudInstanceTypeId = account.LicenseConfig.CloudInstanceType,
                IsLauncher = command.IsLauncher,
                IsSiteMaster = command.SiteId.HasValue,
                CreationMailSent = false,
                MailTo = account.Contact.Email,
                RdpUsers = JsonConvert.SerializeObject(new { }),
                ClassId = account.ClassId,
                Managed = account.Managed,
                Region = machineConfig.Region,
                VmImageName = machineConfig.VmImageName,
                SiteName = site?.UrlFriendlyName,
                SiteMasterUrl = site == null ? null : AccountUtils.GenerateSiteMasterUrl(account.UrlFriendlyName, site.UrlFriendlyName, account.MachineConfig.EnableSsl),
                States = new List<State>() { newState },
                SampleData = machineConfig.IncludeSampleData
            };

            if (site != null)
                site.Machine = machine;

            Context.Set<Machine>().Add(machine);

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
