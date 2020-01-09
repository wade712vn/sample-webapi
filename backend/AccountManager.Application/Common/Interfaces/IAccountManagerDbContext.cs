using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain;

namespace AccountManager.Application.Common.Interfaces
{
    public interface IAccountManagerDbContext
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        Database Database { get; }

        Task<int> SaveChangesAsync(CancellationToken token, out IEnumerable<EntityChangeLog> changes);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();

        DbContextTransaction GetTransaction();
    }
}
