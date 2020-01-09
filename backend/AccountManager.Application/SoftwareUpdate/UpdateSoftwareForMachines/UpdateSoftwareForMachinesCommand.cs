using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AccountManager.Common;
using AccountManager.Domain;
using MediatR;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class UpdateSoftwareForMachinesCommand : CommandBase
    {
        public List<long> Machines { get; set; }

        public long Id { get; set; }

        public string LauncherVersionMode { get; set; }
        public VersionInfo LauncherHash { get; set; }

        public string SiteMasterVersionMode { get; set; }
        public VersionInfo SiteMasterHash { get; set; }

        public string ClientVersionMode { get; set; }
        public VersionInfo ClientHash { get; set; }

        public string ReportingVersionMode { get; set; }
        public VersionInfo ReportingHash { get; set; }

        public string PdfExportVersionMode { get; set; }
        public VersionInfo PdfExportHash { get; set; }

        public string RelExportVersionMode { get; set; }
        public VersionInfo RelExportHash { get; set; }

        public string PopulateVersionMode { get; set; }
        public VersionInfo PopulateHash { get; set; }

        public string DeployerVersionMode { get; set; }
        public VersionInfo DeployerHash { get; set; }

        public string LinkwareVersionMode { get; set; }
        public VersionInfo LinkwareHash { get; set; }

        public string MainLibraryMode { get; set; }
        public long[] MainLibraryFiles { get; set; }
        public string AccountLibraryMode { get; set; }
        public long? AccountLibraryFile { get; set; }
    }
}