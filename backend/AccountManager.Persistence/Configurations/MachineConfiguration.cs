using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;

namespace AccountManager.Persistence.Configurations
{
    public class MachineConfiguration : EntityTypeConfigurationBase<Machine>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "AccountId", "account" },
            { "ClassId", "class" },
            { "CloudInstanceTypeId", "cloud_instance_type" },
            { "IsSiteMaster", "is_sitemaster" },
            { "SiteMasterUrl", "sitemaster_url" },
            { "CloudBackupsSiteMaster", "cloud_backups_sitemaster" },
        };

        public MachineConfiguration()
        {
            ToTable("machine", "machine");

            HasKey(e => e.Id);

            MapProperies();

            HasOptional(e => e.Account)
                .WithMany(e => e.Machines)
                .HasForeignKey(e => e.AccountId);

            HasOptional(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId);

            HasRequired(e => e.CloudInstanceType)
                .WithMany()
                .HasForeignKey(e => e.CloudInstanceTypeId);

            HasMany(e => e.CloudInstances)
                .WithRequired(e => e.Machine)
                .HasForeignKey(e => e.MachineId);

            HasMany(e => e.Operations)
                .WithRequired(e => e.Machine)
                .HasForeignKey(e => e.MachineId);

            HasMany(e => e.Messages)
                .WithRequired(e => e.Machine)
                .HasForeignKey(e => e.MachineId);
        }
    }

    public class MessageConfiguration : EntityTypeConfigurationBase<Message>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "MachineId", "machine" },
        };

        public MessageConfiguration()
        {
            ToTable("message", "machine");

            HasKey(e => e.Id);

            MapProperies();
        }
    }

    public class ClassConfiguration : EntityTypeConfigurationBase<Class>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "IsProduction", null },
        };

        public ClassConfiguration()
        {
            ToTable("class", "machine");

            HasKey(e => e.Id);

            MapProperies();
        }
    }

    public class StateConfiguration : EntityTypeConfigurationBase<State>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "SiteMaster", "sitemaster" },
            { "PdfExport", "pdfexport" },
            { "RelExport", "relexport" },
            { "MachineId", "machine" },
            { "SiteMasterBackup", "sitemaster_backup" },
        };

        public StateConfiguration()
        {
            ToTable("state", "machine");

            HasKey(e => e.Id);

            MapProperies();

            HasOptional(e => e.Machine)
                .WithMany(e => e.States)
                .HasForeignKey(e => e.MachineId);
        }
    }

    public class HistoricalDesiredStateConfiguration : EntityTypeConfigurationBase<HistoricalDesiredState>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "SiteMaster", "sitemaster" },
            { "PdfExport", "pdfexport" },
            { "RelExport", "relexport" },
            { "MachineId", "machine" },
            { "SiteMasterBackup", "sitemaster_backup" },
        };

        public HistoricalDesiredStateConfiguration()
        {
            ToTable("historical_desired_state", "machine");

            HasKey(e => e.Id);

            MapProperies();

            HasOptional(e => e.Machine)
                .WithMany()
                .HasForeignKey(e => e.MachineId);
        }
    }

    public class CloudInstanceConfiguration : EntityTypeConfigurationBase<CloudInstance>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "HostName", "hostname" },
            { "MachineId", "machine" },
            { "SiteMasterPopulated", "sitemaster_populated" },
        };

        public CloudInstanceConfiguration()
        {
            ToTable("cloud_instance", "machine");

            HasKey(e => e.Id);

            MapProperies();
        }
    }

    public class OperationConfiguration : EntityTypeConfigurationBase<Operation>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "MachineId", "machine" },
            { "TypeName", "type" },
        };

        public OperationConfiguration()
        {
            ToTable("operation", "machine");

            HasKey(e => e.Id);

            MapProperies();

            HasRequired(x => x.Type)
                .WithMany()
                .HasForeignKey(x => x.TypeName);
        }
    }

    public class UserOperationConfiguration : EntityTypeConfigurationBase<UserOperation>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "MachineId", "machine" },
            { "TypeName", "type" },
        };

        public UserOperationConfiguration()
        {
            ToTable("user_operation", "machine");

            HasKey(e => e.Id);

            MapProperies();

            HasRequired(x => x.Type)
                .WithMany()
                .HasForeignKey(x => x.TypeName);
        }
    }

    public class OperationTypeConfiguration : EntityTypeConfigurationBase<OperationType>
    {
        public OperationTypeConfiguration()
        {
            ToTable("operation_type", "machine");

            HasKey(e => e.Name);

            MapProperies();
        }
    }

    public class UserOperationTypeConfiguration : EntityTypeConfigurationBase<UserOperationType>
    {
        public UserOperationTypeConfiguration()
        {
            ToTable("user_operation_type", "machine");

            HasKey(e => e.Name);

            MapProperies();
        }
    }
}
