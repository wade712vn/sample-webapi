using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;

namespace AccountManager.Application.Models.Dto
{
    public class AccountDto
    {
        public AccountDto()
        {
            BackupConfig = new BackupConfigDto();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string UrlFriendlyName { get; set; }
        public long? ClassId { get; set; }
        public bool Managed { get; set; }
        public bool AutoTest { get; set; }
        public bool WhiteGlove { get; set; }
        public bool IsTemplate { get; set; }
        public string Customer { get; set; }
        public ClassDto Class { get; set; }
        public ContactDto Contact { get; set; }
        public BillingDto Billing { get; set; }
        public LicenseConfigDto LicenseConfig { get; set; }
        public MachineConfigDto MachineConfig { get; set; }
        public BackupConfigDto BackupConfig { get; set; }
        public ICollection<IdleScheduleDto> IdleSchedules { get; set; }
        public ICollection<SiteDto> Sites { get; set; }

        
    }

    public class IdleScheduleDto
    {
        public long Id { get; set; }
        public DateTimeOffset StopAt { get; set; }
        public int ResumeAfter { get; set; }

        public AccountDto Account { get; set; }
    }

    public class BillingDto
    {
        public double Amount { get; set; }
        public string Period { get; set; }
    }

    public class BackupConfigDto
    {
        public BackupConfigDto()
        {
            Times = new DateTimeOffset[] { };
            WeeklyBackupDay = 0;
            MonthlyBackupDay = 0;
        }

        public long Id { get; set; }

        public int DailyDaysToRetain { get; set; }
        public BackupEolAction DailyEolAction { get; set; }

        public int WeeklyDaysToRetain { get; set; }
        public BackupEolAction WeeklyEolAction { get; set; }
        public int WeeklyBackupDay{ get; set; }

        public int MonthlyDaysToRetain { get; set; }
        public BackupEolAction MonthlyEolAction { get; set; }
        public int MonthlyBackupDay { get; set; }

        public bool IsTemplate { get; set; }
        public string Name { get; set; }
        public DateTimeOffset[] Times { get; set; }
    }

    public enum BackupEolAction
    {
        Delete,
        MoveToGlacier
    }
}
