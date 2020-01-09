using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Contact
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }

        public Account Account { get; set; }
    }
}
