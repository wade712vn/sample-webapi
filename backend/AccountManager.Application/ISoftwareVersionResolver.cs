using AccountManager.Domain;
using AccountManager.Domain.Constants;

namespace AccountManager.Application
{
    public interface ISoftwareVersionResolver
    {
        VersionInfo Resolve(string software, string versionMode, VersionInfo version, string currentVersion = null, string defaultVersion = null);
        long[] GetLibraryFiles(long[] fileIds, string mode, long[] currentFileIds);
    }
}