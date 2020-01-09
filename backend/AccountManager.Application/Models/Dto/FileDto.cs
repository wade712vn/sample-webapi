using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Library;

namespace AccountManager.Application.Models.Dto
{
    public class FileDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string AddedBy { get; set; }
        public string Url { get; set; }
        public bool? InS3 { get; set; }
        public double Size { get; set; }

        public bool LctSigned { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public ICollection<PackageDto> Packages { get; set; }

        public string PackageName
        {
            get
            {
                if (PackageType == "Package")
                {
                    var package = Packages.FirstOrDefault(x => x.Name != "base");
                    return package?.Name;
                }

                if (PackageType == "Library")
                    return "Multiple";

                return null;
            }
        }

        public string PackageType
        {
            get
            {
                if (Name.StartsWith("Library_"))
                    return "Library";

                return Name.StartsWith("Package_") ? "Package" : null;
            }
        }

        public string Group => Regex.Replace(Name, @"_base\d+|_\d{8}_\d{6}", string.Empty);

        public string Date
        {
            get
            {
                var match = Regex.Match(Name, @"\d{8}_\d{6}");
                if (!match.Success)
                    return null;

                if (DateTime.TryParseExact(match.Value, "yyyyMMdd_HHmmss", null, DateTimeStyles.None, out DateTime date))
                    return date.ToString("yyyy/MM/dd HH:mm:ss");

                return null;

            }
        }

        public ReleaseStage ReleaseStage { get; set; }
    }

    
}
