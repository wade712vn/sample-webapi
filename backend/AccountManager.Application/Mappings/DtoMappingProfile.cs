using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Audit;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using Newtonsoft.Json;

namespace AccountManager.Application.Mappings
{
    public class DtoMappingProfile : ProfileBase
    {
        public DtoMappingProfile()
        {
            CreateMap<Account, AccountListingDto>()
                .ForMember(d => d.Class, o => o.MapFrom(src => src.Class != null ? src.Class.Name : string.Empty))
                .ForMember(d => d.ExpirationTime, o => o.MapFrom(src => src.LicenseConfig == null ? default(DateTimeOffset) : src.LicenseConfig.ExpirationTime))
                .ForMember(d => d.CloudCredits, o => o.MapFrom(src => src.LicenseConfig == null ? 0 : src.LicenseConfig.CloudCredits))
                .ForMember(d => d.BillingPeriod, o => o.MapFrom(src => src.Billing == null ? string.Empty : src.Billing.Period.ToString()))
                .ForMember(d => d.InstancePolicy, o => o.MapFrom(src => src.LicenseConfig == null ? string.Empty : src.LicenseConfig.InstancePolicy.ToString().ToSentenceCase()));

            CreateMap<Account, AccountDto>()
                .ForMember(d => d.BackupConfig, o => o.Condition(src => src.BackupConfig != null));

            CreateMap<Customer, CustomerDto>();

            CreateMap<Class, ClassDto>();

            CreateMap<Contact, ContactDto>();
            CreateMap<Billing, BillingDto>()
                .ForMember(d => d.Period, o => o.MapFrom(s => s.Period.ToString()));

            CreateMap<string, VersionInfo>().ConvertUsing<StringVersionInfoConverter>();

            CreateMap<MachineConfig, MachineConfigDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name ?? (s.Account == null ? null : s.Account.Name)))
                .ForMember(d => d.LauncherVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.LauncherVersionMode, s.LauncherHash, s.IsTemplate)))
                .ForMember(d => d.ReportingVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.ReportingVersionMode, s.ReportingHash, s.IsTemplate)))
                .ForMember(d => d.PdfExportVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.PdfExportVersionMode, s.PdfExportHash, s.IsTemplate)))
                .ForMember(d => d.SiteMasterVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.SiteMasterVersionMode, s.SiteMasterHash, s.IsTemplate)))
                .ForMember(d => d.ClientVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.ClientVersionMode, s.ClientHash, s.IsTemplate)))
                .ForMember(d => d.RelExportVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.RelExportVersionMode, s.RelExportHash, s.IsTemplate)))
                .ForMember(d => d.DeployerVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.DeployerVersionMode, s.DeployerHash, s.IsTemplate)))
                .ForMember(d => d.PopulateVersionMode, opts => opts.MapFrom(s => MapVersionMode(s.PopulateVersionMode, s.PopulateHash, s.IsTemplate)))

                .ForMember(d => d.MainLibraryMode, opts => opts.MapFrom(s => s.IsTemplate ? s.MainLibraryMode : (s.MainLibraryFiles.DeserializeArray<long>().Any() ? "Select" : "None")))
                .ForMember(d => d.MainLibraryFileId, opts => opts.MapFrom(s => s.MainLibraryFile))
                .ForMember(d => d.AccountLibraryMode, opts => opts.MapFrom(s => s.IsTemplate ? s.AccountLibraryMode : (s.AccountLibraryFile.HasValue ? "Select" : "None")))
                .ForMember(d => d.AccountLibraryFileId, opts => opts.MapFrom(s => s.AccountLibraryFile))
                .ForMember(d => d.MainLibraryFileIds, opts => opts.MapFrom(s => s.MainLibraryFiles.DeserializeArray<long>()))
                .ForMember(d => d.MainLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.AccountLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.MainLibraryFiles, opts => opts.Ignore());
                

            CreateMap<LicenseConfig, LicenseConfigDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name ?? (s.Account == null ? null : s.Account.Name)))
                .ForMember(d => d.InstancePolicy, o => o.MapFrom(s => s.InstancePolicy.ToString()))
                .ForMember(d => d.Features, o => o.MapFrom(s => JsonConvert.DeserializeObject<string[]>(s.Features)))
                .ForMember(d => d.ReportingCategories, o => o.MapFrom(s => JsonConvert.DeserializeObject<string[]>(s.ReportingCategories)));

            CreateMap<BackupConfig, BackupConfigDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name ?? (s.Account == null ? null : s.Account.Name)))
                .ForMember(d => d.Times, o => o.MapFrom(s => DeserializeBackupTimes(s.Times)));

            CreateMap<Site, SiteDto>();
            
            CreateMap<CloudInstance, CloudInstanceDto>();
            CreateMap<CloudInstanceType, CloudInstanceTypeDto>();
            CreateMap<MmaInstance, MmaInstanceDto>();

            CreateMap<CloudBaseImage, CloudBaseImageDto>();

            CreateMap<Machine, MachineDto>()
                .ForMember(dest => dest.CloudInstanceTypeId, opts => opts.MapFrom(src => src.CloudInstanceTypeId))
                .ForMember(dest => dest.CloudBackupsLauncher, opts => opts.MapFrom(src => src.CloudBackupsLauncher.DeserializeArray<string>()))
                .ForMember(dest => dest.CloudBackupsSiteMaster, opts => opts.MapFrom(src => src.CloudBackupsSiteMaster.DeserializeArray<string>()));

            CreateMap<Message, MessageDto>();
            CreateMap<State, StateDto>()
                .ForMember(d => d.LibraryFileIds, opts => opts.MapFrom(s => s.LibraryFiles.DeserializeArray<long>()))
                .ForMember(d => d.LibraryFileId, opts => opts.MapFrom(s => s.LibraryFile))
                .ForMember(d => d.AccountLibraryFileId, opts => opts.MapFrom(s => s.AccountLibraryFile))
                .ForMember(d => d.AccountLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFiles, opts => opts.Ignore());

            CreateMap<HistoricalDesiredState, StateDto>()
                .ForMember(d => d.LibraryFileIds, opts => opts.MapFrom(s => s.LibraryFiles.DeserializeArray<long>()))
                .ForMember(d => d.LibraryFileId, opts => opts.MapFrom(s => s.LibraryFile))
                .ForMember(d => d.AccountLibraryFileId, opts => opts.MapFrom(s => s.AccountLibraryFile))
                .ForMember(d => d.AccountLibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFile, opts => opts.Ignore())
                .ForMember(d => d.LibraryFiles, opts => opts.Ignore());

            CreateMap<Commit, CommitDto>();
            CreateMap<Repo, RepoDto>();

            CreateMap<File, FileDto>();

            CreateMap<IdleSchedule, IdleScheduleDto>();

            CreateMap<Operation, OperationDto>()
                .ForMember(d => d.TypeName, opts => opts.MapFrom(src => src.TypeName));

            CreateMap<OperationType, OperationTypeDto>();

            CreateMap<Account, AccountRef>();
            CreateMap<Machine, MachineRef>();
        }

        private string MapVersionMode(string versionMode, string hash, bool isTemplate)
        {
            return isTemplate ? versionMode : (new VersionInfo(hash).IsNone() ? "None" : "Select");
        }
    }
}
