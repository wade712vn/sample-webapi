using AccountManager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.Application.Caching.Providers
{
    public interface IMachineCacheProvider
    {
        Task<IEnumerable<OperationType>> ListOperationTypes();
    }
}