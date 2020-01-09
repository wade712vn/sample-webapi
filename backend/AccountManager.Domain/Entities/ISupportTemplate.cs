using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain.Entities
{
    public interface ISupportTemplate : IEntity
    {
        string Name { get; set; }
        bool IsTemplate { get; set; }
        bool IsPublic { get; set; }
    }
}
