using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AccountManager.Common;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AccountManager.Application.Accounts.Commands.UpdateMachine
{
    public class UpdateMachineCommand : CommandBase
    {
        public static IEnumerable<PropertyInfo> Patchables { get; }

        static UpdateMachineCommand()
        {
            Patchables = typeof(UpdateMachineCommand).GetProperties().Where(x => x.PropertyType.IsGenericType &&
                                                                    x.PropertyType.GetGenericTypeDefinition() ==
                                                                    typeof(Patch<>));
        }

        public long AccountId { get; set; }
        public long MachineId { get; set; }
        public Patch<long?> ClassId { get; set; }
        public Patch<bool> Managed { get; set; }
        public Patch<bool?> ManualMaintenance { get; set; }
        public Patch<bool> Turbo { get; set; }
        
    }
}
