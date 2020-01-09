using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;

namespace AccountManager.Persistence.Configurations
{
    public class AccountConfiguration : EntityTypeConfigurationBase<Account>
    {
        public AccountConfiguration()
        {
            ToTable("account", "account");
            
            HasKey(e => e.Id);

            Property(e => e.Id).HasColumnName("id");
            Property(e => e.Name).HasColumnName("name");
            Property(e => e.Description).HasColumnName("description");
            Property(e => e.UrlFriendlyName).HasColumnName("url_friendly_name");
            Property(e => e.LauncherUrl).HasColumnName("launcher_url");
            Property(e => e.License).HasColumnName("license");

            Property(e => e.IsDeleted).HasColumnName("is_deleted");
            Property(e => e.IsActive).HasColumnName("is_active");
            Property(e => e.Managed).HasColumnName("managed");
            Property(e => e.AutoTest).HasColumnName("auto_test");
            Property(e => e.WhiteGlove).HasColumnName("white_glove");

            Property(e => e.IsTemplate).HasColumnName("is_template");
            Property(e => e.IsPublic).HasColumnName("is_public");
            Property(e => e.Customer).HasColumnName("customer");

            Property(e => e.ClassId).HasColumnName("class");

            Property(e => e.Creator).HasColumnName("creator");
            Property(e => e.CreationTime).HasColumnName("creation_time");

            HasOptional(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId);
        }
    }

    public class ContactConfiguration : EntityTypeConfigurationBase<Contact>
    {
        public ContactConfiguration()
        {
            ToTable("contact", "account");

            HasKey(e => e.Id);

            Property(e => e.Id).HasColumnName("id");
            Property(e => e.Name).HasColumnName("name");
            Property(e => e.Phone1).HasColumnName("phone_1");
            Property(e => e.Phone2).HasColumnName("phone_2");
            Property(e => e.Email).HasColumnName("email");

            HasRequired(e => e.Account)
                .WithRequiredDependent(e => e.Contact)
                .Map(a => a.MapKey("account"));
        }
    }

    public class LicenseConfigConfiguration : EntityTypeConfigurationBase<LicenseConfig>
    {
        public LicenseConfigConfiguration()
        {
            ToTable("license_config", "account");

            HasKey(e => e.Id);

            MapProperies();

            HasOptional(e => e.Account)
                .WithOptionalDependent(e => e.LicenseConfig)
                .Map(a => a.MapKey("account"));
        }
    }

    public class MachineConfigConfiguration : EntityTypeConfigurationBase<MachineConfig>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>()
        {
            { "SiteMasterHash", "sitemaster_hash" },
            { "SiteMasterVersionMode", "sitemaster_version_mode" },
            { "MainLibraryFile", "main_library_file" },
            { "MainLibraryFileIds", "main_library_files" },
            { "AccountLibraryFile", "account_library_file" },
        };

        public MachineConfigConfiguration()
        {
            ToTable("machine_config", "account");

            HasKey(e => e.Id);

            MapProperies();

            HasOptional(e => e.Account)
                .WithOptionalDependent(e => e.MachineConfig)
                .Map(a => a.MapKey("account"));
        }
    }

    public class BillingConfiguration : EntityTypeConfigurationBase<Billing>
    {
        public BillingConfiguration()
        {
            ToTable("billing", "account");

            HasKey(e => e.Id);

            Property(e => e.Id).HasColumnName("id");
            Property(e => e.Amount).HasColumnName("amount");
            Property(e => e.Period).HasColumnName("period");

            HasRequired(e => e.Account)
                .WithRequiredDependent(e => e.Billing)
                .Map(a => a.MapKey("account"));
        }
    }

    public class KeysConfiguration : EntityTypeConfigurationBase<Keys>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>()
        {
            { "SqlExportPass", "sqlexport_pass" },
        };

        public KeysConfiguration()
        {
            ToTable("keys", "account");

            HasKey(e => e.Id);

            MapProperies();

            HasRequired(e => e.Account)
                .WithOptional(e => e.Keys)
                .Map(a => a.MapKey("account"));
        }
    }

    public class SiteConfiguration : EntityTypeConfigurationBase<Site>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
        };

        public SiteConfiguration()
        {
            ToTable("site", "account");

            HasKey(e => e.Id);

            Property(e => e.Id).HasColumnName("id");
            Property(e => e.AccountId).HasColumnName("account");
            Property(e => e.Name).HasColumnName("name");
            Property(e => e.MachineId).HasColumnName("machine");
            
            Property(e => e.UrlFriendlyName).HasColumnName("url_friendly_name");
            Property(e => e.CloudInstanceType).HasColumnName("cloud_instance_type");

            HasRequired(e => e.Account)
                .WithMany(e => e.Sites)
                .HasForeignKey(e => e.AccountId);

            HasOptional(e => e.Machine)
                .WithMany()
                .HasForeignKey(x => x.MachineId);
        }
    }

    public class BackupConfigConfiguration : EntityTypeConfigurationBase<BackupConfig>
    {
        public BackupConfigConfiguration()
        {
            ToTable("backup_config", "account");

            HasKey(e => e.Id);

            MapProperies();

            HasOptional(e => e.Account)
                .WithOptionalDependent(e => e.BackupConfig)
                .Map(a => a.MapKey("account"));
        }
    }

    public class IdleScheduleConfiguration : EntityTypeConfigurationBase<IdleSchedule>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>()
        {
            { "AccountId", "account" },
        };

        public IdleScheduleConfiguration()
        {
            ToTable("idle_schedule", "account");

            HasKey(e => e.Id);

            MapProperies();

            HasRequired(e => e.Account)
                .WithMany(e => e.IdleSchedules)
                .HasForeignKey(a => a.AccountId);
        }
    }
  
}
