using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Persistence.Configurations;

namespace AccountManager.Persistence
{
    public class AccountManagerDbContext : DbContext, IAccountManagerDbContext
    {
        public AccountManagerDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
            Configuration.LazyLoadingEnabled = false;
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var mappingTypes = assembly.GetTypes().Where(x => 
                    x.BaseType != null &&
                    x.BaseType.IsGenericType &&
                    x.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfigurationBase<>));

            foreach (var mappingType in mappingTypes)
            {
                dynamic mapping = Activator.CreateInstance(mappingType);
                modelBuilder.Configurations.Add(mapping);
            }

            base.OnModelCreating(modelBuilder);
        }

        public Task<int> SaveChangesAsync(CancellationToken token, out IEnumerable<EntityChangeLog> changes)
        {
            changes = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).SelectMany(x =>
            {
                var entityChanges = new List<EntityChangeLog>();
                foreach (var prop in x.OriginalValues.PropertyNames)
                {
                    var originalValue = x.OriginalValues[prop]?.ToString();
                    var currentValue = x.CurrentValues[prop]?.ToString();
                    if (originalValue != currentValue)
                    {
                        entityChanges.Add(new EntityChangeLog
                        {
                            EntityName = x.Entity.GetType().Name,
                            PropertyName = prop,
                            OldValue = x.OriginalValues[prop],
                            NewValue = x.CurrentValues[prop],
                        });
                    }
                }

                return entityChanges;
            }).ToList();

            return SaveChangesAsync(token);
        }

        public DbContextTransaction GetTransaction()
        {
            return Database.CurrentTransaction ?? Database.BeginTransaction();
        }
    }
}
