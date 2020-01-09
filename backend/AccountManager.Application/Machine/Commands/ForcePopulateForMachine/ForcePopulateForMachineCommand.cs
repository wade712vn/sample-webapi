using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountManager.Application.Commands.ForcePopulateForMachine
{
    public class ForcePopulateForMachineCommand : CommandBase
    {
        public long Id { get; set; }

        public bool PopulateLauncher { get; set; }
        public bool PopulateSiteMaster { get; set; }
        public string AltString { get; set; }
    }

    public class ForcePopulateForMachineCommandHandler : IRequestHandler<ForcePopulateForMachineCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public ForcePopulateForMachineCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ForcePopulateForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Machine>()
                .Include(x => x.CloudInstances)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.Id);

            if (!machine.CloudInstances.Any())
                throw new CommandException("Machine has no instance");

            foreach (var instance in machine.CloudInstances)
            {
                if (command.PopulateLauncher)
                    instance.LauncherPopulated = false;

                if (command.PopulateSiteMaster)
                    instance.SiteMasterPopulated = false;

                instance.AltString = command.AltString;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
