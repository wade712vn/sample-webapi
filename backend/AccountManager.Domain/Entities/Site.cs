using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Site
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UrlFriendlyName { get; set; }
        public long CloudInstanceType { get; set; }
        public long AccountId { get; set; }

        public long? MachineId { get; set; }

        public Account Account { get; set; }
        public Machine Machine { get; set; }
        
    }
}
