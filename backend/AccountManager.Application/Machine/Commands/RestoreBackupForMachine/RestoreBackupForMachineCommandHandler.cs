using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Commands.RestoreBackupForMachine
{
    public class RestoreBackupForMachineCommandHandler : CommandHandlerBase<RestoreBackupForMachineCommand, Unit>
    {

        public RestoreBackupForMachineCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(RestoreBackupForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            var desiredState = await Context.Set<State>()
                .FirstOrDefaultAsync(x => x.MachineId == command.Id && x.Desired, cancellationToken);

            if (desiredState == null)
                throw new CommandException("Machine has no desired state");

            if (!command.SiteMasterBackupFile.IsNullOrWhiteSpace())
            {
                desiredState.SiteMasterBackup = command.SiteMasterBackupFile;
            }

            if (!command.LauncherBackupFile.IsNullOrWhiteSpace())
            {
                desiredState.LauncherBackup = command.LauncherBackupFile;
            }

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent()
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.Restore

            }, cancellationToken);

            return Unit.Value;
        }
    }
}