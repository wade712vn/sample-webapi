using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Commands;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.StopMachine
{
    public class StopMachineCommandHandler : CommandHandlerBase<StopMachineCommand, Unit>
    {

        public StopMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(StopMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = Context.Set<Machine>().Find(command.Id);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            machine.Stop = true;
            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent()
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.StopMachine

            }, cancellationToken);

            return Unit.Value;
        }
    }
}