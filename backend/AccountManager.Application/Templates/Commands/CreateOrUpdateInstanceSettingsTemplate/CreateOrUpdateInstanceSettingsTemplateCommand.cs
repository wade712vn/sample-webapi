using AccountManager.Domain;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate
{
    public class CreateOrUpdateInstanceSettingsTemplateCommand : IRequest<long>
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public bool IsPublic { get; set; }

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

        public string AccountLibraryMode { get; set; }
        public long? AccountLibraryFile { get; set; }
        public string MainLibraryMode { get; set; }
        public long[] MainLibraryFiles { get; set; }
        public string Region { get; set; }
        public string VmImageName { get; set; }
    }
}
