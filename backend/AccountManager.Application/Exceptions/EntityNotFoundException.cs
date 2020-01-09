using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}
