using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Public;

namespace AccountManager.Persistence.Configurations
{
    public class CloudInstanceTypeConfiguration : EntityTypeConfigurationBase<CloudInstanceType>
    {
        public CloudInstanceTypeConfiguration()
        {
            ToTable("cloud_instance_type", "public");

            HasKey(e => e.Id);

            Property(e => e.Id).HasColumnName("id");
            Property(e => e.CloudCode).HasColumnName("cloud_code");
            Property(e => e.CloudCreditCost).HasColumnName("cloud_credit_cost");
            Property(e => e.Name).HasColumnName("name");
            Property(e => e.StorageSize).HasColumnName("storage_size");
        }
    }

    public class CloudRegionConfiguration : EntityTypeConfigurationBase<CloudRegion>
    {
        public CloudRegionConfiguration()
        {
            ToTable("cloud_region", "public");

            HasKey(e => e.CloudCode);

            Property(e => e.CloudCode).HasColumnName("cloud_code");
            Property(e => e.Name).HasColumnName("name");
        }
    }

    public class CloudBaseImageConfiguration : EntityTypeConfigurationBase<CloudBaseImage>
    {
        public CloudBaseImageConfiguration()
        {
            ToTable("cloud_base_image", "public");

            HasKey(e => e.Name);

            Property(e => e.Name).HasColumnName("name");
            Property(e => e.Description).HasColumnName("description");
        }
    }

    public class MmaInstanceConfiguration : EntityTypeConfigurationBase<MmaInstance>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "MachineClassId", "machine_class" },
        };

        public MmaInstanceConfiguration()
        {
            ToTable("mma_instance", "public");

            HasKey(e => e.Id);

            MapProperies();

            HasRequired(e => e.MachineClass)
                .WithMany(e => e.MmaInstances)
                .HasForeignKey(e => e.MachineClassId);
        }
    }
}
