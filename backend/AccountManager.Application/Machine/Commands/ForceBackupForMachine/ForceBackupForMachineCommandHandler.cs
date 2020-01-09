using System;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Commands.ForceBackupForMachine;

namespace AccountManager.Application.Commands.ForceBackupForMachine
{
    public class ForceBackupForMachineCommandHandler : CommandHandlerBase<ForceBackupForMachineCommand, Unit>
    {

        public ForceBackupForMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(ForceBackupForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            machine.Turbo = true;
            machine.NextBackupTime = DateTimeOffset.Now.Add(new TimeSpan(0, command.TimeToBackup, 0));

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent()
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.Backup

            }, cancellationToken);

            return Unit.Value;
        }
    }
}
