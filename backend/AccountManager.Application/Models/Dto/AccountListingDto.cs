using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;

namespace AccountManager.Application.Models.Dto
{
    public class AccountListingDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlFriendlyName { get; set; }
        public bool IsActive { get; set; }
        public bool Managed { get; set; }
        public string Class { get; set; }
        public long ClassId { get; set; }
        public bool AutoTest { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public string Creator { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public string LauncherUrl { get; set; }

        public string Customer { get; set; }

        public string BillingPeriod { get; set; }
        public string InstancePolicy { get; set; }
        public int CloudCredits { get; set; }

        public string BundleVersion { get; set; }
    }
}
