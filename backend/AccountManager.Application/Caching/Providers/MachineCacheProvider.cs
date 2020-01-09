using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;

namespace AccountManager.Application.Caching.Providers
{
    public class MachineCacheProvider : IMachineCacheProvider
    {
        public Task<IEnumerable<OperationType>> ListOperationTypes()
        {
            throw new NotImplementedException();
        }
    }
}
