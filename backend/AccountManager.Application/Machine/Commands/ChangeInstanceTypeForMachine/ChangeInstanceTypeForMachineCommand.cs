using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;

namespace AccountManager.Application.Commands.ChangeInstanceTypeForMachine
{
    public class ChangeInstanceTypeForMachineCommand : CommandBase
    {
        public long Id { get; set; }
        public long InstanceTypeId { get; set; }
    }

    public class ChangeInstanceTypeForMachineCommandHandler : IRequestHandler<ChangeInstanceTypeForMachineCommand>
    {
        private readonly IMediator _mediator;
        private readonly IAccountManagerDbContext _context;

        public ChangeInstanceTypeForMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<Unit> Handle(ChangeInstanceTypeForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            machine.CloudInstanceTypeId = command.InstanceTypeId;

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
