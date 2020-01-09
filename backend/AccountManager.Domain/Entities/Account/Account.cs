using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Account : ISupportTemplate
    {
        private ICollection<Site> _sites;
        private ICollection<Machine> _machines;
        private ICollection<IdleSchedule> _idleSchedules;

        public long Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlFriendlyName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string Creator { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public string License { get; set; }
        public string LauncherUrl { get; set; }
        public bool Managed { get; set; }
        public bool AutoTest { get; set; }
        public bool IsTemplate { get; set; }
        public bool IsPublic { get; set; }

        public bool WhiteGlove { get; set; }

        public long? ClassId { get; set; }

        public Class Class { get; set; }
        public string Customer { get; set; }
        public Contact Contact { get; set; }
        public Billing Billing { get; set; }
        public MachineConfig MachineConfig { get; set; }
        public LicenseConfig LicenseConfig { get; set; }
        public BackupConfig BackupConfig { get; set; }
        public Keys Keys { get; set; }

        public ICollection<Site> Sites
        {
            get => _sites ?? (_sites = new List<Site>());
            set => _sites = value;
        }

        public ICollection<Machine> Machines
        {
            get => _machines ?? (_machines = new List<Machine>());
            set => _machines = value;
        }

        public ICollection<IdleSchedule> IdleSchedules
        {
            get => _idleSchedules ?? (_idleSchedules = new List<IdleSchedule>());
            set => _idleSchedules = value;
        }
    }
}
