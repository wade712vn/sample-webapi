using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Commands;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.StartMachine
{
    public class StartMachineCommandHandler : CommandHandlerBase<StartMachineCommand, Unit>
    {

        public StartMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(StartMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = Context.Set<Machine>().Find(command.Id);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            machine.Stop = false;
            machine.Idle = false;
            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent()
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.StartMachine

            }, cancellationToken);

            return Unit.Value;
        }
    }
}