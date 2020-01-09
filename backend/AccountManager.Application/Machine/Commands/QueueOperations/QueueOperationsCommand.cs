using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Commands.QueueOperations
{
    public class QueueOperationsCommand : CommandBase
    {
        public long Id { get; set; }
        public string[] Operations { get; set; }
    }

    public class QueueOperationsCommandHandler : IRequestHandler<QueueOperationsCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public QueueOperationsCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(QueueOperationsCommand command, CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Machine>()
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            var runningOperation = await _context.Set<Operation>().FirstOrDefaultAsync(x => x.MachineId == machine.Id && (x.Active || x.Status.ToLower() == "running"), cancellationToken);
            if (runningOperation != null)
                throw new CommandException();

            var operationTypeName = command.Operations.FirstOrDefault();
            var operationType = await _context.Set<OperationType>()
                .FirstOrDefaultAsync(x => x.Name == operationTypeName, cancellationToken);

            if (operationType == null || !(operationType.CanBeManual.HasValue && operationType.CanBeManual.Value))
                throw new CommandException();

            var forcedOperation = new Operation()
            {
                Type = operationType,
                Active = true,
                Timestamp = DateTimeOffset.Now,
                Status = "FORCED",
                MachineId = command.Id,
                TypeName = operationTypeName
            };

            _context.Set<Operation>().Add(forcedOperation);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
