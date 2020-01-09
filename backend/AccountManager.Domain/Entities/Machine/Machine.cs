using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Machine
    {
        public Machine()
        {

        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Params { get; set; }
        public string LauncherUrl { get; set; }
        public string SiteName { get; set; }
        public string MailTo { get; set; }
        public string SiteMasterUrl { get; set; }
        public bool Terminate { get; set; }
        public bool Stop { get; set; }
        public bool NeedsAdmin { get; set; }
        public int FailCounter { get; set; }
        public bool? Dummy { get; set; }
        public string RdpUsers { get; set; }
        public long CloudInstanceTypeId { get; set; }
        public bool IsLauncher { get; set; }
        public bool IsSiteMaster { get; set; }
        public bool? Managed { get; set; }
        public bool? ManualMaintenance { get; set; }
        public bool? CreationMailSent { get; set; }
        public string Region { get; set; }
        public string VmImageName { get; set; }
        public bool? Idle { get; set; }
        public bool? Turbo { get; set; }
        public long? ClassId { get; set; }
        public bool? SampleData { get; set; }

        public DateTimeOffset? NextStartTime { get; set; }
        public DateTimeOffset? NextStopTime { get; set; }
        public DateTimeOffset? NextBackupTime { get; set; }

        public string CloudBackupsLauncher { get; set; }
        public string CloudBackupsSiteMaster { get; set; }

        public long? AccountId { get; set; }

        public Account Account { get; set; }
        public Class Class { get; set; }
        public CloudInstanceType CloudInstanceType { get; set; }
        public ICollection<State> States { get; set; }
        public ICollection<CloudInstance> CloudInstances { get; set; }
        public ICollection<Operation> Operations { get; set; }
        public ICollection<Message> Messages { get; set; }
        
    }
}
