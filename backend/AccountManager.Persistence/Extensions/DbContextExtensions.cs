using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain;

namespace AccountManager.Persistence.Extensions
{
    public static class DbContextExtensions
    {
        public static Task<int> SaveChangesAsync(this DbContext context, CancellationToken token, out IEnumerable<EntityChangeLog> changes)
        {
            changes = context.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).SelectMany(x =>
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

            return context.SaveChangesAsync(token);
        }
    }
}
