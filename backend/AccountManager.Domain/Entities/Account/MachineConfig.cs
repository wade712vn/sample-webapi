using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class MachineConfig : ISupportTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }
        
        public bool ShowInGrafana { get; set; }
        public bool UseSparePool { get; set; }
        public bool IncludeSampleData { get; set; }
        public bool IncludeIrmoData { get; set; }
        public bool UseUniqueKey { get; set; }
        public bool EnableSsl { get; set; }

        public string LauncherVersionMode { get; set; }
        public string LauncherHash { get; set; }

        public string SiteMasterVersionMode { get; set; }
        public string SiteMasterHash { get; set; }

        public string DeployerVersionMode { get; set; }
        public string DeployerHash { get; set; }

        public string ClientVersionMode { get; set; }
        public string ClientHash { get; set; }

        public string ReportingVersionMode { get; set; }
        public string ReportingHash { get; set; }

        public string PdfExportVersionMode { get; set; }
        public string PdfExportHash { get; set; }

        public string RelExportVersionMode { get; set; }
        public string RelExportHash { get; set; }

        public string PopulateVersionMode { get; set; }
        public string PopulateHash { get; set; }

        public string LinkwareVersionMode { get; set; }
        public string LinkwareHash { get; set; }

        public string AccountLibraryMode { get; set; }
        public long? AccountLibraryFile { get; set; }
        public string MainLibraryMode { get; set; }
        public long? MainLibraryFile { get; set; }
        public string MainLibraryFiles { get; set; }
        public string Region { get; set; }
        public string VmImageName { get; set; }

        public bool IsTemplate { get; set; }
        public bool IsPublic { get; set; }

        public Account Account { get; set; }
    }
}
