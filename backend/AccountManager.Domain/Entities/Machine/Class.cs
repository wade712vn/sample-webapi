using AccountManager.Domain.Entities.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public class Class
    {
        public static string[] ProductionClasses = new[]
            {"Internal Production", "Customer Production", "Customer Staging", "On Premises"};

        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<MmaInstance> MmaInstances { get; set; }

        public MmaInstance MmaInstance { get
            {
                return MmaInstances?.FirstOrDefault();
            }
        }

        public bool IsProduction
        {
            get
            {
                if (ProductionClasses.Contains(Name))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
