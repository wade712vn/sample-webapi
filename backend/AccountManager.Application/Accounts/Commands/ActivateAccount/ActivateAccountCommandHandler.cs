using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.ActivateAccount
{
    public class ActivateAccountCommandHandler : IRequestHandler<ActivateAccountCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public ActivateAccountCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ActivateAccountCommand command, CancellationToken cancellationToken)
        {

            var account = await _context.Set<Account>()
                .Include(x => x.Machines)
                .FirstOrDefaultAsync(x => !x.IsDeleted && !x.IsTemplate && x.Id == command.Id, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.Id);

            account.IsActive = true;
            foreach (var machine in account.Machines)
            {
                machine.Stop = command.StartMachines;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}