using AccountManager.Domain;

namespace AccountManager.Application.Models.Dto
{
    public class MachineConfigDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsTemplate { get; set; }

        public bool ShowInGrafana { get; set; }
        public bool UseSparePool { get; set; }
        public bool IncludeSampleData { get; set; }
        public bool IncludeIrmoData { get; set; }
        public bool UseUniqueKey { get; set; }
        public bool EnableSsl { get; set; }

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
        public long? MainLibraryFileId { get; set; }
        public FileDto MainLibraryFile { get; set; }
        public long[] MainLibraryFileIds { get; set; }
        public FileDto[] MainLibraryFiles { get; set; }

        public string AccountLibraryMode { get; set; }
        public long? AccountLibraryFileId { get; set; }
        public FileDto AccountLibraryFile { get; set; }

        public long[] AccountLibraryFileIds => AccountLibraryFileId.HasValue ? new[] { AccountLibraryFileId.Value } : new long[0];

        public FileDto[] AccountLibraryFiles => AccountLibraryFile != null ? new[] { AccountLibraryFile } : new FileDto[0];

        public string Region { get; set; }

        public string VmImageName { get; set; }

        public string BundleVersion { get; set; }
    }


}