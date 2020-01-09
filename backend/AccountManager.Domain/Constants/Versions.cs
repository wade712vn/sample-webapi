using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Constants
{
    public static class Softwares
    {
        public const string Launcher = "launcher";
        public const string SiteMaster = "siteMaster";
        public const string Deployer = "deployer";
        public const string PdfExport = "pdfExport";
        public const string RelationalExport = "relExport";
        public const string Populate = "populate";
        public const string Reporting = "reporting";
        public const string WebClient = "client";
        public const string Linkware = "linkware";
    }

    public static class Versions
    {
        public const string None = "N/A";
    }

    public static class VersionModes
    {
        public const string None = "None";
        public const string Latest = "Latest";
        public const string Skip = "Skip";
    }

    public static class LibraryFiles
    {
        public static long? None = null;
    }

    public static class LibraryFileModes
    {
        public const string None = "";
        public const string Latest = "Latest";
        public const string LatestDev = "Latest Dev";
        public const string LatestProd = "Latest Prod";
        public const string LatestAccount = "Latest Account";
        public const string Skip = "Skip";
        public const string Select = "Select";
        
    }
}
