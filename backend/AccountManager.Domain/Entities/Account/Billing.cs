using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Billing
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public BillingPeriod Period { get; set; }

        public Account Account { get; set; }
    }
}
