using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Library;

namespace AccountManager.Persistence.Configurations
{
    public class FileConfiguration : EntityTypeConfigurationBase<File>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "ReleaseStage", null },
        };

        public FileConfiguration()
        {
            ToTable("file", "library");

            HasKey(e => e.Id);

            HasMany(e => e.Packages)
                .WithOptional(e => e.File)
                .HasForeignKey(e => e.FileId);

            MapProperies();
        }
    }

    public class PackageConfiguration : EntityTypeConfigurationBase<Package>
    {
        protected override IDictionary<string, string> ColumnMappings => new Dictionary<string, string>()
        {
            { "FileId", "file" },
        };

        public PackageConfiguration()
        {
            ToTable("package", "library");

            HasKey(e => e.Id);

            MapProperies();
        }
    }
}
