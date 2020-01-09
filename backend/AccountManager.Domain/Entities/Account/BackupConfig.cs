using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class BackupConfig : ISupportTemplate
    {
        public long Id { get; set; }

        public int DailyDaysToRetain { get; set; }
        public string DailyEolAction { get; set; }

        public int WeeklyBackupDay { get; set; }
        public int WeeklyDaysToRetain { get; set; }
        public string WeeklyEolAction { get; set; }
        
        
        public int MonthlyBackupDay { get; set; }
        public int MonthlyDaysToRetain { get; set; }
        public string MonthlyEolAction { get; set; }

        public bool IsTemplate { get; set; }
        public string Name { get; set; }
        public string Times { get; set; }

        public Account Account { get; set; }
        public bool IsPublic { get; set; }
    }
}
