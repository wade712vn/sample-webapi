using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Models.Dto
{
    public class CloudInstanceDto
    {
        public long Id { get; set; }
        public string CloudId { get; set; }
        public string HostName { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Status { get; set; }
        public string CloudCode { get; set; }
        public bool Active { get; set; }
        public bool? DnsReady { get; set; }
        public bool? RdpReady { get; set; }
        public int? StorageSize { get; set; }
        public string IpAddress { get; set; }

        public bool LauncherPopulated { get; set; }
        public bool SiteMasterPopulated { get; set; }
        public string AltString { get; set; }

        public string GrafanaUid { get; set; }
        public string VmImageName { get; set; }
    }
}
