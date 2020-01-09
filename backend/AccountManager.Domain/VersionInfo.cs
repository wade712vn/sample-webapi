using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Constants;

namespace AccountManager.Domain
{
    public class VersionInfo
    {
        public string Hash { get; set; }
        public string Branch { get; set; }

        public VersionInfo()
        {
        }

        public VersionInfo(string value)
        {
            string hash, branchName;
            if (value == Versions.None)
            {
                branchName = null;
                hash = null;
            }
            else if (!value.IsNullOrWhiteSpace() && (value.Contains("|") || value.Contains("/")))
            {
                var parts = value.Replace(@"/", "|").Split(new[] { '|' }, StringSplitOptions.None);
                branchName = parts[0].Trim();
                hash = parts[1].Trim();
            }
            else
            {
                hash = value;
                branchName = null;
            }

            Hash = hash;
            Branch = branchName;
        }

        public override string ToString()
        {
            if (!Branch.IsNullOrWhiteSpace())
            {
                return $"{Branch}|{Hash}";
            }

            return Hash;
        }
    }

    public static class VersionInfoExtensions
    {
        public static bool IsNone(this VersionInfo versionInfo)
        {
            if (versionInfo == null)
                return true;

            if (versionInfo.Hash.IsNullOrWhiteSpace())
                return true;

            return false;
        }
    }
}
