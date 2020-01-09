using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Commands.RecreateMachine
{
    public class RecreateMachineCommandHandler : CommandHandlerBase<RecreateMachineCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _versionResolver;

        public RecreateMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context, ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<Unit> Handle(RecreateMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = Context.Set<Machine>()
                .Find(command.MachineId);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.MachineId);

            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.MachineConfig)
                .Include(x => x.LicenseConfig)
                .FirstOrDefaultAsync(x => x.Id == machine.AccountId && !x.IsDeleted, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), machine.AccountId);

            machine.Terminate = true;

            var machineConfig = account.MachineConfig;
            var newState = new State()
            {
                Launcher = machine.IsLauncher ? new VersionInfo(machineConfig.LauncherHash).Hash : Versions.None,
                Reporting = machine.IsLauncher ? new VersionInfo(machineConfig.ReportingHash).Hash : Versions.None,
                PdfExport = machine.IsLauncher ? new VersionInfo(machineConfig.PdfExportHash).Hash : Versions.None,
                Client = machine.IsSiteMaster ? new VersionInfo(machineConfig.ClientHash).Hash : Versions.None,
                SiteMaster = machine.IsSiteMaster ? new VersionInfo(machineConfig.SiteMasterHash).Hash : Versions.None,
                RelExport = machine.IsSiteMaster ? new VersionInfo(machineConfig.RelExportHash).Hash : Versions.None,
                Linkware = machine.IsSiteMaster ? new VersionInfo(machineConfig.LinkwareHash).Hash : Versions.None,

                Deployer = new VersionInfo(machineConfig.DeployerHash).Hash,
                Populate = new VersionInfo(machineConfig.PopulateHash).Hash,
                LibraryFile = machineConfig.MainLibraryFile ?? 0,
                LibraryFiles = machineConfig.MainLibraryFiles,
                AccountLibraryFile = machineConfig.AccountLibraryFile ?? 0,

                Locked = false,
                Desired = true,
                SslEnabled = machineConfig.EnableSsl,
            };

            var newMachine = new Machine()
            {
                Name = machine.Name,
                Account = account,
                CloudInstanceTypeId = account.LicenseConfig.CloudInstanceType,
                IsLauncher = machine.IsLauncher,
                CreationMailSent = false,
                IsSiteMaster = machine.IsSiteMaster,
                MailTo = account.Contact.Email,
                RdpUsers = JsonConvert.SerializeObject(new { }),
                ClassId = machine.ClassId,
                Managed = account.Managed,
                Region = machineConfig.Region,
                VmImageName = machine.VmImageName,
                SiteName = machine.SiteName,
                SiteMasterUrl = machine.SiteMasterUrl,
                States = new List<State>() { newState },
                SampleData = machineConfig.IncludeSampleData
            };

            var site = await Context.Set<Site>()
                .Include(x => x.Machine)
                .FirstOrDefaultAsync(x => x.Machine != null && x.Machine.Id == machine.Id, cancellationToken);

            account.Machines.Add(newMachine);

            if (site != null)
            {
                site.Machine = newMachine;
            }

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineRecreatedEvent()
            {
                Actor = command.User,
                Machine = machine,
                Command = command

            }, cancellationToken);

            return Unit.Value;
        }
    }
}