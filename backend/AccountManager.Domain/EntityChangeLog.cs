using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Domain
{
    public class EntityChangeLog
    {
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
