using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.ResetMachineFailureState
{
    public class ResetMachineFailureStateCommandHandler : IRequestHandler<ResetMachineFailureStateCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public ResetMachineFailureStateCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ResetMachineFailureStateCommand request, CancellationToken cancellationToken)
        {
            var machine = _context.Set<Machine>().Find(request.Id);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), request.Id);

            machine.NeedsAdmin = false;
            machine.FailCounter = 0;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}