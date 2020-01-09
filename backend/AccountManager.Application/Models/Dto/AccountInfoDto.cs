using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Models.Dto
{
    public class AccountInfoDto
    {
        public long Id { get; set; }
        public int NeedsAdmin { get; internal set; }
        public int ManualMaintenance { get; internal set; }
        public int Managed { get; internal set; }
        public int Turbo { get; internal set; }
        public int MachineCount { get; set; }
        public int Idle { get; set; }
        public int Stop { get; set; }
    }
}
