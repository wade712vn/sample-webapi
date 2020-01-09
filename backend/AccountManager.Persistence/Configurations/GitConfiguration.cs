using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Git;

namespace AccountManager.Persistence.Configurations
{
    public class RepoConfiguration : EntityTypeConfigurationBase<Repo>
    {
        public RepoConfiguration()
        {
            ToTable("repo", "git");

            HasKey(e => e.Name);

            MapProperies();

            HasMany(e => e.Commits)
                .WithRequired()
                .HasForeignKey(e => e.Repo);
        }
    }

    public class CommitConfiguration : EntityTypeConfigurationBase<Commit>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>()
        {
            { "BranchId", "branch" },
        };

        public CommitConfiguration()
        {
            ToTable("commit", "git");

            HasKey(e => e.Id);

            MapProperies();
        }
    }

    public class BranchConfiguration : EntityTypeConfigurationBase<Branch>
    {
        protected override IDictionary<string, string> ColumnMappings { get; } = new Dictionary<string, string>()
        {
            { "BranchId", "branch" },
        };

        public BranchConfiguration()
        {
            ToTable("branch", "git");

            HasKey(e => e.Id);

            MapProperies();

            HasMany(e => e.Commits)
                .WithRequired(e => e.Branch)
                .HasForeignKey(e => e.BranchId);
        }
    }
}
