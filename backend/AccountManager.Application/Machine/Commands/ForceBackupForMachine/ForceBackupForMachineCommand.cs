using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountManager.Application.Commands.ForceBackupForMachine
{
    public class ForceBackupForMachineCommand : CommandBase
    {
        public long Id { get; set; }
        public int TimeToBackup { get; set; }
    }
}
