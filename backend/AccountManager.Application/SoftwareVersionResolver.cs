using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;

namespace AccountManager.Application
{
    public class SoftwareVersionResolver : ISoftwareVersionResolver
    {
        private readonly IAccountManagerDbContext _context;


        private static readonly IDictionary<string, string> SoftwareRepoMappings = new Dictionary<string, string>()
        {
            { Softwares.Launcher, "launcher" },
            { Softwares.SiteMaster, "sitemaster" },
            { Softwares.WebClient, "client" },
            { Softwares.Deployer, "deployer" },
            { Softwares.Reporting, "reporting" },
            { Softwares.PdfExport, "pdfexport" },
            { Softwares.RelationalExport, "relexport" },
            { Softwares.Populate, "populate" },
        };

        public SoftwareVersionResolver(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public VersionInfo Resolve(string software, string versionMode, VersionInfo version, string currentVersion = null, string defaultVersion = null)
        {
            switch (versionMode)
            {
                case null:
                case VersionModes.None:
                    return new VersionInfo(defaultVersion);
                case VersionModes.Skip:
                    return new VersionInfo(currentVersion);
                case VersionModes.Latest:
                    var repo = SoftwareRepoMappings[software];
                    var branchName = version?.Branch;
                    var query = !branchName.IsNullOrWhiteSpace() ? 
                        _context.Set<Commit>().Include(x => x.Branch).Where(x => x.Branch.Name == branchName) : 
                        _context.Set<Commit>().Include(x => x.Branch).Where(x => x.Branch.Stable);

                    query = query.Where(x => x.Repo == repo);

                    var latestCommit = query.OrderByDescending(x => x.Timestamp).FirstOrDefault();

                    return new VersionInfo { Branch = latestCommit?.Branch.Name, Hash = latestCommit?.ShortHash };

                
                default:
                    return version;
            }
        }

        public long[] GetLibraryFiles(long[] fileIds, string mode, long[] currentFileIds)
        {
            switch (mode)
            {
                case LibraryFileModes.Select:
                    {
                        var libraryFiles = _context.Set<File>().Where(x => fileIds.Contains(x.Id));
                        return libraryFiles.Select(x => x.Id).ToArray();
                    }
                case LibraryFileModes.None:
                    return new long[0];
                case LibraryFileModes.Skip:
                    return currentFileIds;
                case LibraryFileModes.LatestAccount:
                    {
                        var libraryFile = _context.Set<File>().OrderByDescending(x => x.Timestamp).FirstOrDefault(x => x.Type == "Account");
                        return libraryFile == null ? new long[0] : new long[] { libraryFile.Id };
                    }
                case LibraryFileModes.LatestDev:
                    {
                        var libraryFile = _context.Set<File>().OrderByDescending(x => x.Timestamp).FirstOrDefault(x => x.Type == "Dev");
                        return libraryFile == null ? new long[0] : new long[] { libraryFile.Id };
                    }
                case LibraryFileModes.LatestProd:
                    {
                        var libraryFile = _context.Set<File>().OrderByDescending(x => x.Timestamp).FirstOrDefault(x => x.Type == "Prod");
                        return libraryFile == null ? new long[0] : new long[] { libraryFile.Id };
                    }

                default:
                    return new long[0];
            }
        }
    }
}
