using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using Irm.Security;
using Irm.Security.Licensing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AccountManager.Application.License
{
    public static class LicenseUtils
    {
        public static byte[] GenerateLicense(LicenseConfig licenseConfig, string licensePrivateKey)
        {
            var license = new IrmLicense()
            {
                MaxAreaCount = licenseConfig.MaxAreas,
                MaxEquipCount = licenseConfig.MaxEquips,
                MaxCircuitCount = licenseConfig.MaxCircuits,
                MaxSiteCount = licenseConfig.MaxSites,
                MaxSoftwareCount = licenseConfig.MaxSoftwares,
                MaxMainHoleCount = licenseConfig.MaxMainholes,
                MaxUserCount = licenseConfig.MaxUsers,
                MaxPathwayCount = licenseConfig.MaxPathways,
                MaxFaceplateCount = licenseConfig.MaxFaceplates,
                MaxRackCount = licenseConfig.MaxRacks,
                MaxCloudInstanceCredits = licenseConfig.CloudCredits,
                MaxReportUsers = licenseConfig.MaxReportUsers,
                MaxClientUsers = licenseConfig.MaxClientUsers,
                IsActive = licenseConfig.IsActive,
                Expiration = licenseConfig.ExpirationTime?.LocalDateTime ?? DateTime.MaxValue,
                Features = JsonConvert.DeserializeObject<IrmLicense.Feature[]>(licenseConfig.Features),
                ReportCategories = JsonConvert.DeserializeObject<IrmLicense.SuperCategory[]>(licenseConfig.ReportingCategories),
            };

            var signedLicense = DSAProvider.Sign(license, licensePrivateKey);
            var serializationSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-ddTH:mm:ss.fffZ"
            };

            serializationSettings.Converters.Add(new StringEnumConverter());

            var jsonStr = JsonConvert.SerializeObject(signedLicense, serializationSettings);
            var bytes = Encoding.UTF8.GetBytes(jsonStr);

            return bytes;
        }
    }
}
