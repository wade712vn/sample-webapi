using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities.Library
{
    public class File
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string AddedBy { get; set; }
        public string Signature { get; set; }
        public double? Size { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public ICollection<Package> Packages { get; set; }

        public ReleaseStage ReleaseStage
        {
            get
            {
                var match = Regex.Match(Name, @"@(dev|rel)");
                if (match.Success && (match.Value == "@rel"))
                    return ReleaseStage.Released;

                return ReleaseStage.Development;
            }
        }
    }

    public enum ReleaseStage
    {
        Development,
        Released,
    }
}
