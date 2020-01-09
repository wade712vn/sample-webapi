using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Models.Dto
{
    public class CloudInstanceTypeDto
    {
        public long Id { get; set; }
        public string CloudCode { get; set; }
        public int StorageSize { get; set; }
        public string Name { get; set; }
        public int? CloudCreditCost { get; set; }
    }
}
