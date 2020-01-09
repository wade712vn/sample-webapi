using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Exceptions;
using AccountManager.Common;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateMachine
{
    public class UpdateMachineCommandHandler : CommandHandlerBase<UpdateMachineCommand, Unit>
    {
        public UpdateMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = Context.Set<Machine>().Find(command.MachineId);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.MachineId);

            foreach (var patchable in UpdateMachineCommand.Patchables)
            {
                if (!(patchable.GetValue(command) is Patch patch) || !patch.Patchable)
                    continue;

                var targetProperty = typeof(Machine).GetProperty(patchable.Name);
                if (targetProperty == null)
                    continue;

                targetProperty.SetValue(machine, patch.Value);
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            // Add audit log
            await Mediator.Publish(new UserActionNotification()
            {
                User = command.User,
                Machines = new[] { machine },
                Action = "UpdateMachine",
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