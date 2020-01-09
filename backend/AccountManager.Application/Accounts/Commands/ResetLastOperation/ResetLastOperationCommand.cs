using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.ResetLastOperation
{
    public class ResetLastOperationCommand : IRequest
    {
        public long Id { get; set; }
    }

    public class ResetLastOperationCommandHandler : IRequestHandler<ResetLastOperationCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public ResetLastOperationCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ResetLastOperationCommand request, CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), request.Id);

            var activeOperation = await _context.Set<Operation>().OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(x => x.MachineId == request.Id && x.Active, cancellationToken);

            if (activeOperation == null)
                throw new CommandException($"MachineDto {request.Id} has no active operation");

            _context.Set<Operation>().Remove(activeOperation);

            await _context.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(Unit.Value);
        }
    }
}
