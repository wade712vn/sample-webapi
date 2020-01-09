using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccountManager.Common;

namespace AccountManager.Application.Accounts.Commands.BatchUpdateAccounts
{
    public class BatchUpdateAccountsCommand : CommandBase
    {
        public static IEnumerable<PropertyInfo> Patchables { get; }

        static BatchUpdateAccountsCommand()
        {
            Patchables = typeof(BatchUpdateAccountsCommand).GetProperties().Where(x => x.PropertyType.IsGenericType &&
                                                                                 x.PropertyType.GetGenericTypeDefinition() ==
                                                                                 typeof(Patch<>));
        }

        public long[] AccountIds { get; set; }

        public Patch<long?> ClassId { get; set; }
        public Patch<bool> Managed { get; set; }
        public Patch<bool> AutoTest { get; set; }
    }
}
