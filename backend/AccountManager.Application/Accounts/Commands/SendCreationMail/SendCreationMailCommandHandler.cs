using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.SendCreationMail
{
    public class SendCreationMailCommandHandler : IRequestHandler<SendCreationMailCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public SendCreationMailCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SendCreationMailCommand request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Machines)
                .FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountId);

            var launcherMachine = account.Machines.FirstOrDefault(x => x.IsLauncher);
            if (launcherMachine == null)
                throw new CommandException("No launcher machineDto can be found");

            launcherMachine.CreationMailSent = false;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}