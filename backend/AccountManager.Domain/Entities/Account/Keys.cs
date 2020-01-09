using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Keys
    {
        public long Id { get; set; }
        public string ApiPassword { get; set; }
        public string UserTokenPublic { get; set; }
        public string UserTokenPrivate { get; set; }
        public string InterServerPublic { get; set; }
        public string InterServerPrivate { get; set; }
        public string LicensePublic { get; set; }
        public string LicensePrivate { get; set; }
        public string AccountFile { get; set; }
        public byte[] SslPackage { get; set; }
        public string SqlExportPass { get; set; }

        public Account Account { get; set; }
    }
}
