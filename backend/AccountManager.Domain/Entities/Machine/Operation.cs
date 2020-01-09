using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Operation
    {
        public long Id { get; set; }
        public long MachineId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int FailCounter { get; set; }
        public string Params { get; set; }
        public int FallbackLevel { get; set; }
        public string TypeName { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? FinishTime { get; set; }
        public string Output { get; set; }

        public Machine Machine { get; set; }
        public OperationType Type { get; set; }
    }

}
