using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Persistence.Extensions
{
    public static class DatabaseExtensions
    {
        public static DbContextTransaction GetTransaction(this Database database)
        {
            return database.CurrentTransaction ?? database.BeginTransaction();
        }
    }
}
