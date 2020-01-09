using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.StopMachine;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.TerminateMachine
{
    public class TerminateMachineCommandHandler : CommandHandlerBase<TerminateMachineCommand, Unit>
    {
        public TerminateMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(TerminateMachineCommand request, CancellationToken cancellationToken)
        {
            var machine = Context.Set<Machine>().Find(request.Id);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), request.Id);

            machine.Terminate = true;
            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}