using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Key;
using AccountManager.Application.License;
using AccountManager.Application.Utils;
using AccountManager.Common.Extensions;
using AccountManager.Common.Keys;
using AccountManager.Common.Utils;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using AsymmetricKeyPair = AccountManager.Application.Key.AsymmetricKeyPair;

namespace AccountManager.Application.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : CommandHandlerBase<CreateAccountCommand, long>
    {
        private readonly IKeysManager _keysManager;
        private readonly ISoftwareVersionResolver _versionResolver;

        public CreateAccountCommandHandler(IMediator mediator, IAccountManagerDbContext context, IKeysManager keysManager, ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _keysManager = keysManager;
            _versionResolver = versionResolver;
        }

        public override async Task<long> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
        {
            var account = Mapper.Map<Account>(command);
            var machineConfig = account.MachineConfig;

            account.IsActive = true;
            account.CreationTime = DateTimeOffset.Now;
            account.Creator = command.User;

            var userTokenKeyPair = _keysManager.GenerateUserTokenKeyPair(!command.UseUniqueKey);
            var interServerKeyPair = _keysManager.GenerateInterServerKeyPair(!command.UseUniqueKey);
            var licenseKeyPair = _keysManager.GenerateLicenseKeyPair(!command.UseUniqueKey);

            var apiPassword = StringUtils.GeneratePassword(16);
            account.Keys = new Keys()
            {
                UserTokenPrivate = userTokenKeyPair.PrivateKey,
                UserTokenPublic = userTokenKeyPair.PublicKey,
                InterServerPrivate = interServerKeyPair.PrivateKey,
                InterServerPublic = interServerKeyPair.PublicKey,
                LicensePrivate = licenseKeyPair.PrivateKey,
                LicensePublic = licenseKeyPair.PublicKey,
                ApiPassword = apiPassword,
                AccountFile = $"{account.UrlFriendlyName}:{apiPassword}".ToBase64(),
                SqlExportPass = StringUtils.GeneratePassword(24)
            };

            var licenseBytes = LicenseUtils.GenerateLicense(account.LicenseConfig, account.Keys.LicensePrivate);
            account.License = Convert.ToBase64String(licenseBytes);

            var launcherVersion = _versionResolver.Resolve(Softwares.Launcher, command.LauncherVersionMode, command.LauncherHash);
            machineConfig.LauncherHash = launcherVersion?.ToString();

            var reportingVersion = _versionResolver.Resolve(Softwares.Reporting, command.ReportingVersionMode, command.ReportingHash);
            machineConfig.ReportingHash = reportingVersion?.ToString();

            var clientVersion = _versionResolver.Resolve(Softwares.WebClient, command.ClientVersionMode, command.ClientHash);
            machineConfig.ClientHash = clientVersion?.ToString();

            var siteMasterVersion = _versionResolver.Resolve(Softwares.SiteMaster, command.SiteMasterVersionMode, command.SiteMasterHash);
            machineConfig.SiteMasterHash = siteMasterVersion?.ToString();

            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances).FirstOrDefaultAsync(x => x.Id == command.ClassId, cancellationToken);
            var deployerVersion = _versionResolver.Resolve(Softwares.Deployer, command.DeployerVersionMode, command.DeployerHash, mmaClass?.MmaInstance?.DeployerVer);
            machineConfig.DeployerHash = deployerVersion?.ToString();

            var pdfExportVersion = _versionResolver.Resolve(Softwares.PdfExport, command.PdfExportVersionMode, command.PdfExportHash);
            machineConfig.PdfExportHash = pdfExportVersion?.ToString();

            var relExportVersion = _versionResolver.Resolve(Softwares.RelationalExport, command.RelExportVersionMode, command.RelExportHash);
            machineConfig.RelExportHash = relExportVersion?.ToString();

            var populateVersion = _versionResolver.Resolve(Softwares.Populate, command.PopulateVersionMode, command.PopulateHash);
            machineConfig.PopulateHash = populateVersion?.ToString();

            var linkwareVersion = _versionResolver.Resolve(Softwares.Linkware, command.LinkwareVersionMode, command.LinkwareHash);
            machineConfig.LinkwareHash = linkwareVersion?.ToString();

            var mainLibraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles, command.MainLibraryMode, machineConfig.MainLibraryFiles.DeserializeArray<long>());
            machineConfig.MainLibraryFiles = mainLibraryFiles.Serialize();
            machineConfig.MainLibraryFile = mainLibraryFiles.FirstOrDefault();
            machineConfig.MainLibraryMode = machineConfig.MainLibraryFiles.Any()
                ? LibraryFileModes.Select : LibraryFileModes.None;

            var accountLibraryFiles = _versionResolver.GetLibraryFiles(command.AccountLibraryFile.HasValue ?
                    new[] { command.AccountLibraryFile.Value } : new long[0],
                command.AccountLibraryMode,
                machineConfig.AccountLibraryFile.HasValue ? new[] { machineConfig.AccountLibraryFile.Value } : new long[0]);

            if (accountLibraryFiles.Any())
            {
                machineConfig.AccountLibraryFile = accountLibraryFiles.First();
                machineConfig.AccountLibraryMode = LibraryFileModes.Select;
            }
            else
            {
                machineConfig.AccountLibraryFile = null;
                machineConfig.AccountLibraryMode = LibraryFileModes.None;
            }

            var state = new State()
            {
                Launcher = launcherVersion.IsNone() ? Versions.None : launcherVersion.Hash,
                Reporting = reportingVersion.IsNone() ? Versions.None : reportingVersion.Hash,
                PdfExport = pdfExportVersion.IsNone() ? Versions.None : pdfExportVersion.Hash,
                Client = clientVersion.IsNone() ? Versions.None : clientVersion.Hash,
                SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Hash,
                Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Hash,

                RelExport = relExportVersion.IsNone() ? Versions.None : relExportVersion.Hash,
                Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Hash,

                Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Hash,

                LibraryFile = machineConfig.MainLibraryFile ?? 0,
                LibraryFiles = machineConfig.MainLibraryFiles,
                AccountLibraryFile = machineConfig.AccountLibraryFile ?? 0,
                
                Locked = false,
                Desired = true,
                SslEnabled = command.EnableSsl,
                MonitoringEnabled = command.ShowInGrafana
            };

            var baseDomainUrl = "";
            var machine = new Machine()
            {
                CloudInstanceTypeId = command.CloudInstanceType,
                IsLauncher = true,
                CreationMailSent = false,
                LauncherUrl = $"{(command.EnableSsl ? "https" : "http")}://{account.UrlFriendlyName}.{baseDomainUrl}",
                MailTo = command.ContactEmail,
                RdpUsers = JsonConvert.SerializeObject(new {}),
                States = new List<State>() { state },
                ClassId = command.ClassId,
                Managed = command.Managed,
                Region = command.Region,
                VmImageName = command.VmImageName,
                SampleData = command.IncludeSampleData,
                Turbo = true
            };
            account.Machines.Add(machine);

            var serverInstancePolicy = command.InstancePolicy;
            if (serverInstancePolicy == ServerInstancePolicy.AllInOne)
            {
                var site = new Site()
                {
                    Name = command.SiteMasterName,
                    UrlFriendlyName = command.SiteMasterUrlFriendlyName,
                    CloudInstanceType = command.CloudInstanceType
                };
                account.Sites.Add(site);

                machine.IsSiteMaster = true;
                machine.SiteName = site.UrlFriendlyName;
                machine.SiteMasterUrl = AccountUtils.GenerateSiteMasterUrl(account.UrlFriendlyName, site.UrlFriendlyName, command.EnableSsl);
                machine.Name = AccountUtils.GenerateMachineName(command.UrlFriendlyName, command.InstancePolicy);

                site.Machine = machine;
            }
            else
            {
                machine.Name = AccountUtils.GenerateMachineName(command.UrlFriendlyName, command.InstancePolicy, "launcher");

                state.SiteMaster = Versions.None;
                state.Linkware = Versions.None;
                state.Client = Versions.None;
                state.RelExport = Versions.None;
            }

            Context.Set<Account>().Add(account);

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new AccountCreatedEvent()
            {
                User = command.User,
                Account = account ,
                Command = command

            }, cancellationToken);

            return account.Id;
        }
    }
}