using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Common;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Library;
using MediatR;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class UpdateSoftwareForMachinesCommandHandler : CommandHandlerBase<UpdateSoftwareForMachinesCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _versionResolver;

        
        public UpdateSoftwareForMachinesCommandHandler(IMediator mediator, IAccountManagerDbContext context, ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<Unit> Handle(UpdateSoftwareForMachinesCommand command, CancellationToken cancellationToken)
        {
            var machines = await Context.Set<Machine>().Include(x => x.States).Where(x => command.Machines.Contains(x.Id)).ToListAsync(cancellationToken);

            foreach (var machine in machines)
            {
                var desiredState = machine?.States.FirstOrDefault(x => x.Desired);
                if (desiredState == null)
                    continue;

                if (machine.IsLauncher)
                {
                    var launcherVersion = _versionResolver.Resolve(Softwares.Launcher, command.LauncherVersionMode, command.LauncherHash, desiredState.Launcher);
                    var reportingVersion = _versionResolver.Resolve(Softwares.Reporting, command.ReportingVersionMode, command.ReportingHash, desiredState.Reporting);
                    var pdfExportVersion = _versionResolver.Resolve(Softwares.PdfExport, command.PdfExportVersionMode, command.PdfExportHash, desiredState.PdfExport);

                    desiredState.Launcher = launcherVersion.IsNone() ? Versions.None : launcherVersion.Hash;
                    desiredState.Reporting = reportingVersion.IsNone() ? Versions.None : reportingVersion.Hash;
                    desiredState.PdfExport = pdfExportVersion.IsNone() ? Versions.None : pdfExportVersion.Hash;
                }
                else
                {
                    desiredState.Launcher = Versions.None;
                    desiredState.Reporting = Versions.None;
                    desiredState.PdfExport = Versions.None;
                }

                if (machine.IsSiteMaster)
                {
                    var clientVersion = _versionResolver.Resolve(Softwares.WebClient, command.ClientVersionMode, command.ClientHash, desiredState.Client);
                    var siteMasterVersion = _versionResolver.Resolve(Softwares.SiteMaster, command.SiteMasterVersionMode, command.SiteMasterHash, desiredState.SiteMaster);
                    var relExportVersion = _versionResolver.Resolve(Softwares.RelationalExport, command.RelExportVersionMode, command.RelExportHash, desiredState.RelExport);
                    var linkwareVersion = _versionResolver.Resolve(Softwares.Linkware, command.LinkwareVersionMode, command.LinkwareHash, desiredState.Linkware);

                    desiredState.SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Hash;
                    desiredState.RelExport = relExportVersion.IsNone() ? Versions.None : relExportVersion.Hash;
                    desiredState.Client = clientVersion.IsNone() ? Versions.None : clientVersion.Hash;
                    desiredState.Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Hash;
                }
                else
                {
                    desiredState.SiteMaster = Versions.None;
                    desiredState.RelExport = Versions.None;
                    desiredState.Client = Versions.None;
                    desiredState.Linkware = Versions.None;
                }

                var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances).FirstOrDefaultAsync(x => x.Id == machine.ClassId);
                var deployerVersion = _versionResolver.Resolve(Softwares.Deployer, command.DeployerVersionMode, command.DeployerHash, desiredState.Deployer, mmaClass?.MmaInstance?.DeployerVer);
                var populateVersion = _versionResolver.Resolve(Softwares.Populate, command.PopulateVersionMode, command.PopulateHash, desiredState.Populate);

                desiredState.Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Hash;
                desiredState.Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Hash;

                var libraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles, command.MainLibraryMode, desiredState.LibraryFiles.DeserializeArray<long>());
                desiredState.LibraryFiles = libraryFiles.Serialize();
                desiredState.LibraryFile = libraryFiles.Any() ? libraryFiles.FirstOrDefault(x => {
                    var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                    return file != null;
                }) : 0;

                desiredState.AccountLibraryFile = _versionResolver.GetLibraryFiles(command.AccountLibraryFile.HasValue ?
                    new[] { command.AccountLibraryFile.Value } : new long[0], command.AccountLibraryMode, desiredState.AccountLibraryFile.HasValue ? new[] { desiredState.AccountLibraryFile.Value } : new long[0]).FirstOrDefault(x => {
                        var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                        return file != null;
                    });

                machine.Turbo = true;
            }

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new SoftwareUpdatedEvent()
            {
                Actor = command.User,
                Machines = machines,
                Command = command

            }, cancellationToken);

            return Unit.Value;
        }
    }
}
