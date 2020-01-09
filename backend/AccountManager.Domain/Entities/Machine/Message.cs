using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Message
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public long ExpiresAfter { get; set; }

        public long MachineId { get; set; }
        public Machine Machine { get; set; }
    }
}
