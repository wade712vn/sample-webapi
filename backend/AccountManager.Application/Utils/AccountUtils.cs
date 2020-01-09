using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;

namespace AccountManager.Application.Utils
{
    public static class AccountUtils
    {
        public static string GenerateMachineName(string accountUrlName, ServerInstancePolicy instancePolicy, string appName = null)
        {
            const string prefix = "MMA";
            switch (instancePolicy)
            {
                case ServerInstancePolicy.AllInOne:
                    return $"{prefix} {accountUrlName} AllInOne";
                case ServerInstancePolicy.InstancePerSiteMaster:
                    return $"{prefix} {accountUrlName} IPSM {appName}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(instancePolicy), instancePolicy, null);
            }
        }

        public static string GenerateSiteMasterUrl(string accountUrlName, string siteUrlName, bool enableSsl)
        {
            var baseDomainUrl = "";
            return
                $"{(enableSsl ? "https" : "http")}://{siteUrlName}.{accountUrlName}.{baseDomainUrl}";
        }
    }
}
