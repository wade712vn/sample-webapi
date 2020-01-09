using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.UpdateMachine;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Common;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.BatchUpdateMachines
{
    public class BatchUpdateMachinesCommandHandler : CommandHandlerBase<BatchUpdateMachinesCommand, Unit>
    {

        public BatchUpdateMachinesCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(BatchUpdateMachinesCommand command, CancellationToken cancellationToken)
        {

            var machines = new List<Machine>();
            foreach (var machineId in command.MachineIds)
            {
                var machine = await Context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == machineId, cancellationToken);

                foreach (var patchable in BatchUpdateMachinesCommand.Patchables)
                {
                    if (!(patchable.GetValue(command) is Patch patch) || !patch.Patchable)
                        continue;

                    var targetProperty = typeof(Machine).GetProperty(patchable.Name);
                    if (targetProperty == null)
                        continue;

                    targetProperty.SetValue(machine, patch.Value);
                }

                machines.Add(machine);
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            // 
            // Add audit log
            await Mediator.Publish(new UserActionNotification()
            {
                User = command.User,
                Machines = machines.ToArray(),
                Action = "BatchUpdateMachines",
                Data = new
                {
                    Params = command,
                    Changes = changes
                }

            }, cancellationToken);

            return Unit.Value;

        }
    }
}