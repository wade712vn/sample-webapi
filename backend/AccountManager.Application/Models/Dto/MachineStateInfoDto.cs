using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Models.Dto
{
    public class MachineStateInfoDto
    {
        public IEnumerable<StateDto> States { get; set; }
        public string BundleVersion { get; set; }
    }
}
