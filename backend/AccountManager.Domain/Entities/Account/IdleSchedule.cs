using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class IdleSchedule
    {
        public long Id { get; set; }
        public DateTimeOffset StopAt { get; set; }
        public int ResumeAfter { get; set; }
        public long AccountId { get; set; }

        public Account Account { get; set; }
    }
}
