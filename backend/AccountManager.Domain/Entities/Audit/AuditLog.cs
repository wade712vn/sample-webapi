using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities.Audit
{
    public class AuditLog : MongoEntityBase
    {
        public string User { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }
        public AccountRef[] Accounts { get; set; }
        public MachineRef[] Machines { get; set; }

        public dynamic MetaData { get; set; }
    }

    public class MachineRef
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class AccountRef
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UrlFriendlyName { get; set; }
    }
}
