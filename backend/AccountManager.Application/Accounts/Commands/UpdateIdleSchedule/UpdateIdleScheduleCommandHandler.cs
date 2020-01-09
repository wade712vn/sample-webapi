using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateIdleSchedule
{
    public class UpdateIdleScheduleCommandHandler : CommandHandlerBase<UpdateIdleScheduleCommand, Unit>
    {
        public UpdateIdleScheduleCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateIdleScheduleCommand command, CancellationToken cancellationToken)
        {
            var account = Context.Set<Account>()
                .Include(x => x.IdleSchedules)
                .FirstOrDefault(x => !x.IsDeleted && x.Id == command.AccountId);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);

            
            foreach (var idleSchedule in account.IdleSchedules.ToArray())
            {
                Context.Set<IdleSchedule>().Remove(idleSchedule);
                account.IdleSchedules.Remove(idleSchedule);
            }

            foreach (var idleScheduleDto in command.IdleSchedules)
            {
                var newIdleSchedule = Mapper.Map<IdleSchedule>(idleScheduleDto);
                account.IdleSchedules.Add(newIdleSchedule);
            }

            var machines = await Context.Set<Machine>()
                .Where(x => x.AccountId.HasValue && x.AccountId.Value == account.Id)
                .ToListAsync(cancellationToken);

            foreach (var machine in machines)
            {
                machine.NextStartTime = null;
                machine.NextStopTime = null;
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            await Mediator.Publish(new UserActionNotification()
            {
                User = command.User,
                Accounts = new[] { account },
                Action = "PushIdleSchedule",
                Data = new
                {
                    Params = command,
                    Changes = changes
                }

            }, cancellationToken);

            return Unit.Value;
        }

        
    }
}