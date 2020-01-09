using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Library;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts
{
    public class UpdateSoftwareForAccountsCommandHandler : CommandHandlerBase<UpdateSoftwareForAccountsCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _versionResolver;

        public UpdateSoftwareForAccountsCommandHandler(IMediator mediator, IAccountManagerDbContext context, ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<Unit> Handle(UpdateSoftwareForAccountsCommand command, CancellationToken cancellationToken)
        {
            var transaction = Context.GetTransaction();
            try
            {
                var accounts = await Context.Set<Account>().Include(x => x.MachineConfig)
                    .Where(x => command.Accounts.Contains(x.Id)).ToListAsync(cancellationToken);
                foreach (var account in accounts)
                {
                    var machineConfig = account.MachineConfig;

                    if (machineConfig == null)
                        continue;

                    var launcherVersion = _versionResolver.Resolve(Softwares.Launcher, command.LauncherVersionMode,
                        command.LauncherHash, machineConfig.LauncherHash);
                    machineConfig.LauncherHash = launcherVersion?.ToString();

                    var reportingVersion = _versionResolver.Resolve(Softwares.Reporting, command.ReportingVersionMode,
                        command.ReportingHash, machineConfig.ReportingHash);
                    machineConfig.ReportingHash = reportingVersion?.ToString();

                    var clientVersion = _versionResolver.Resolve(Softwares.WebClient, command.ClientVersionMode, 
                        command.ClientHash, machineConfig.ClientHash);
                    machineConfig.ClientHash = clientVersion?.ToString();

                    var siteMasterVersion = _versionResolver.Resolve(Softwares.SiteMaster, command.SiteMasterVersionMode, 
                        command.SiteMasterHash, machineConfig.SiteMasterHash);
                    machineConfig.SiteMasterHash = siteMasterVersion?.ToString();


                    var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances).FirstOrDefaultAsync(x => x.Id == machineConfig.Account.ClassId, cancellationToken);
                    var deployerVersion = _versionResolver.Resolve(Softwares.Deployer, command.DeployerVersionMode,
                        command.DeployerHash, machineConfig.DeployerHash, mmaClass?.MmaInstance.DeployerVer);
                    machineConfig.DeployerHash = deployerVersion?.ToString();

                    var pdfExportVersion = _versionResolver.Resolve(Softwares.PdfExport, command.PdfExportVersionMode,
                        command.PdfExportHash, machineConfig.PdfExportHash);
                    machineConfig.PdfExportHash = pdfExportVersion?.ToString();

                    var relExportVersion = _versionResolver.Resolve(Softwares.RelationalExport, command.RelExportVersionMode, 
                        command.RelExportHash, machineConfig.RelExportHash);
                    machineConfig.RelExportHash = relExportVersion?.ToString();

                    var populateVersion = _versionResolver.Resolve(Softwares.Populate, command.PopulateVersionMode,
                        command.PopulateHash, machineConfig.PopulateHash);
                    machineConfig.PopulateHash = populateVersion?.ToString();

                    var linkwareVersion = _versionResolver.Resolve(Softwares.Populate, command.LinkwareVersionMode,
                        command.LinkwareHash, machineConfig.LinkwareHash);
                    machineConfig.LinkwareHash = linkwareVersion?.ToString();

                    var mainLibraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles, command.MainLibraryMode, machineConfig.MainLibraryFiles.DeserializeArray<long>());
                    machineConfig.MainLibraryFiles = mainLibraryFiles.Serialize();
                    machineConfig.MainLibraryFile = mainLibraryFiles.FirstOrDefault(x => {
                        var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                        return file != null;
                    });

                    machineConfig.MainLibraryMode = machineConfig.MainLibraryFiles.Any()
                        ? LibraryFileModes.Select : LibraryFileModes.None;

                    var accountLibraryFiles = _versionResolver.GetLibraryFiles(command.AccountLibraryFile.HasValue ?
                        new[] { command.AccountLibraryFile.Value } : new long[0], 
                        command.AccountLibraryMode, 
                        machineConfig.AccountLibraryFile.HasValue ? new[] { machineConfig.AccountLibraryFile.Value } : new long[0]);

                    if (accountLibraryFiles.Any())
                    {
                        machineConfig.AccountLibraryFile = accountLibraryFiles.FirstOrDefault(x => {
                            var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                            return file != null;
                        });

                        machineConfig.AccountLibraryMode = LibraryFileModes.Select;
                    }
                    else
                    {
                        machineConfig.AccountLibraryFile = null;
                        machineConfig.AccountLibraryMode = LibraryFileModes.None;
                    }

                    var machines = Context.Set<Machine>()
                        .Include(x => x.States)
                        .Where(x => x.AccountId == account.Id).ToList();

                    foreach (var machine in machines)
                    {
                        await UpdateMachine(machine, command, cancellationToken);
                    }
                }

                await Context.SaveChangesAsync(cancellationToken);

                transaction.Commit();

                await Mediator.Publish(new SoftwareUpdatedEvent()
                {
                    Actor = command.User,
                    Accounts = accounts,
                    Command = command

                }, cancellationToken);

                return Unit.Value;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        private async Task UpdateMachine(Machine machine, UpdateSoftwareForAccountsCommand command, CancellationToken cancellationToken)
        {
            var states = await Context.Set<State>().Where(x => x.MachineId == machine.Id).ToListAsync(cancellationToken);
            var desiredState = states.FirstOrDefault(x => x.Desired);
            if (desiredState == null)
                return;

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
    }
}
