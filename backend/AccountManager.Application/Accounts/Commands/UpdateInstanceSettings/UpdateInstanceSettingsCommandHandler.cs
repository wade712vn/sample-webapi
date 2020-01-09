using AccountManager.Application.Exceptions;
using AccountManager.Application.License;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts;
using AccountManager.Domain.Constants;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain;

namespace AccountManager.Application.Accounts.Commands.UpdateInstanceSettings
{
    public class UpdateInstanceSettingsCommandHandler : CommandHandlerBase<UpdateInstanceSettingsCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _versionResolver;

        public UpdateInstanceSettingsCommandHandler(IMediator mediator, IAccountManagerDbContext context, ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<Unit> Handle(UpdateInstanceSettingsCommand command, CancellationToken cancellationToken)
        {

            var transaction = Context.GetTransaction();
            try
            {
                var account = Context.Set<Account>()
                    .Include(x => x.MachineConfig)
                    .Include(x => x.Machines)
                    .FirstOrDefault(x => !x.IsDeleted && x.Id == command.AccountId);

                if (account == null)
                    throw new EntityNotFoundException(nameof(Account), command.AccountId);

                var machineConfig = account.MachineConfig;

                machineConfig.ShowInGrafana = command.ShowInGrafana;
                machineConfig.UseSparePool = command.UseSparePool;
                machineConfig.IncludeSampleData = command.IncludeSampleData;
                machineConfig.IncludeIrmoData = command.IncludeIrmoData;
                machineConfig.VmImageName = command.VmImageName;

                await UpdateSoftwareVersions(machineConfig, command, cancellationToken);

                var machineList = account.Machines;

                foreach (var machine in account.Machines)
                {
                    await UpdateMachine(machine, command, cancellationToken);
                }

                await Context.SaveChangesAsync(cancellationToken, out var changes);

                transaction.Commit();

                await Mediator.Publish(new InstanceSettingsPushedEvent()
                {
                    Actor = command.User,
                    Account = account,
                    Command = command,
                    Changes = changes

                }, cancellationToken);

                return Unit.Value;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task UpdateSoftwareVersions(MachineConfig machineConfig, UpdateInstanceSettingsCommand command, CancellationToken cancellationToken)
        {
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


            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances).FirstOrDefaultAsync(x => x.Id == machineConfig.Account.ClassId);
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
        }

        private async Task UpdateMachine(Machine machine, UpdateInstanceSettingsCommand command, CancellationToken cancellationToken)
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

            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances).FirstOrDefaultAsync(x => x.Id == machine.ClassId, cancellationToken);
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

            desiredState.MonitoringEnabled = command.ShowInGrafana;

            machine.VmImageName = command.VmImageName;
            machine.Turbo = true;
        }
    }
}
