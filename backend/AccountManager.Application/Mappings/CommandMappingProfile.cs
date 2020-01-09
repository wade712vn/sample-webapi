using System;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.UpdateAccount;
using AccountManager.Application.Accounts.Commands.UpdateBackupSettings;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines;
using AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateGeneralTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate;
using AccountManager.Application.Templates.Commands.CreateOrUpdateLicenseTemplate;
using AccountManager.Common;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AutoMapper;

namespace AccountManager.Application.Mappings
{
    public class CommandMappingProfile : ProfileBase
    {
        public CommandMappingProfile()
        {
            CreateMap<CreateAccountCommand, LicenseConfig>()
                .ForMember(dest => dest.ReportingCategories, opts => opts.MapFrom(src => Serialize(src.ReportingCategories ?? new string[0])))
                .ForMember(dest => dest.Name, opts => opts.Ignore())
                .ForMember(dest => dest.Features, opts => opts.MapFrom(src => Serialize(src.Features ?? new string[0])));

            CreateMap<CreateAccountCommand, MachineConfig>()
                .ForMember(dest => dest.MainLibraryFiles, opts => opts.MapFrom(src => (src.MainLibraryFiles ?? new long[0]).Serialize()))
                .ForMember(dest => dest.Name, opts => opts.Ignore());

            CreateMap<CreateAccountCommand, BackupConfig>()
                .ForMember(dest => dest.Times, opts => opts.MapFrom(src => SerializeForPostgresTimestampArray(src.Times)))
                .ForMember(dest => dest.Name, opts => opts.Ignore());

            CreateMap<CreateAccountCommand, Account>()
                .ForMember(dest => dest.Contact, opts => opts.MapFrom(src => new Contact() { Name = src.ContactName, Email = src.ContactEmail, Phone1 = src.ContactPhone1, Phone2 = src.ContactPhone2 }))
                .ForMember(dest => dest.Billing, opts => opts.MapFrom(src => new Billing() { Period = src.BillingPeriod, Amount = src.BillingAmount }))
                .ForMember(dest => dest.LicenseConfig, opts => opts.MapFrom(src => src))
                .ForMember(dest => dest.MachineConfig, opts => opts.MapFrom(src => src))
                .ForMember(dest => dest.BackupConfig, opts => opts.MapFrom(src => src));

            CreateMap<CreateOrUpdateLicenseTemplateCommand, LicenseConfig>()
                .ForMember(dest => dest.ReportingCategories, opts => opts.MapFrom(src => Serialize(src.ReportingCategories ?? new string[0])))
                .ForMember(dest => dest.Features, opts => opts.MapFrom(src => Serialize(src.Features ?? new string[0])));

            CreateMap<CreateOrUpdateInstanceSettingsTemplateCommand, MachineConfig>()
                .ForMember(dest => dest.MainLibraryFiles, opts => opts.MapFrom(src => (src.MainLibraryFiles ?? new long[0]).Serialize()));

            CreateMap<CreateOrUpdateBackupSettingsTemplateCommand, BackupConfig>()
                .ForMember(dest => dest.Times, opts => opts.MapFrom(src => SerializeForPostgresTimestampArray(src.Times)));


            CreateMap<CreateOrUpdateGeneralTemplateCommand, Account>()
                .ForMember(dest => dest.Contact, opts => opts.MapFrom(src => new ContactTemplate() { Name = src.ContactName, Email = src.ContactEmail, Phone1 = src.ContactPhone1, Phone2 = src.ContactPhone2 }))
                .ForMember(dest => dest.Billing, opts => opts.MapFrom(src => new BillingTemplate() { Period = src.BillingPeriod, Amount = src.BillingAmount }))
                .ForMember(dest => dest.Keys, opts => opts.Ignore())
                .ForMember(dest => dest.LicenseConfig, opts => opts.Ignore())
                .ForMember(dest => dest.MachineConfig, opts => opts.Ignore());

            CreateMap<CreateOrUpdateGeneralTemplateCommand, LicenseConfig>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.ReportingCategories, opts => opts.MapFrom(src => Serialize(src.ReportingCategories ?? new string[0])))
                .ForMember(dest => dest.Features, opts => opts.MapFrom(src => Serialize(src.Features ?? new string[0])));

            CreateMap<CreateOrUpdateGeneralTemplateCommand, MachineConfig>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.MainLibraryFiles, opts => opts.MapFrom(src => (src.MainLibraryFiles ?? new long[0]).Serialize()));

            CreateMap<CreateOrUpdateGeneralTemplateCommand, BackupConfig>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.Times, opts => opts.MapFrom(src => SerializeForPostgresTimestampArray(src.Times)));


            CreateMap<ContactTemplate, Contact>();
            CreateMap<BillingTemplate, Billing>();

            CreateMap<UpdateAccountCommand, Account>()
                .ForMember(dest => dest.Contact, opts => opts.MapFrom(src => src))
                .ForMember(dest => dest.Billing, opts => opts.MapFrom(src => src));

            CreateMap<UpdateAccountCommand, Contact>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.ContactEmail))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.ContactName))
                .ForMember(dest => dest.Phone1, opts => opts.MapFrom(src => src.ContactPhone1))
                .ForMember(dest => dest.Phone2, opts => opts.MapFrom(src => src.ContactPhone2));

            CreateMap<UpdateAccountCommand, Billing>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.Amount, opts => opts.MapFrom(src => src.BillingAmount))
                .ForMember(dest => dest.Period, opts => opts.MapFrom(src => src.BillingPeriod));

            CreateMap<UpdateLicenseSettingsCommand, LicenseConfig>()
                .ForMember(dest => dest.ReportingCategories, opts => opts.MapFrom(src => Serialize(src.ReportingCategories ?? new string[0])))
                .ForMember(dest => dest.Features, opts => opts.MapFrom(src => Serialize(src.Features ?? new string[0])));

            CreateMap<UpdateInstanceSettingsCommand, UpdateSoftwareForAccountsCommand>()
                .ForMember(dest => dest.Accounts, opts => opts.Ignore());

            CreateMap<UpdateSoftwareForAccountsCommand, UpdateSoftwareForMachinesCommand>();

            CreateMap<UpdateBackupSettingsCommand, BackupConfig>()
                .ForMember(dest => dest.Times, opts => opts.MapFrom(src => SerializeForPostgresTimestampArray(src.Times)));

            
        }
    }
}
