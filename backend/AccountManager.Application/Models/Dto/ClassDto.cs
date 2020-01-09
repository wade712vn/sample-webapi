using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Models.Dto
{
    public class ClassDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsProduction { get; set; }

        public MmaInstanceDto MmaInstance
        {
            get; set;
        }
    }
}
