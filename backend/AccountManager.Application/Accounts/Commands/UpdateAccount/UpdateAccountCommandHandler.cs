using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Exceptions;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateAccount
{
    public class UpdateAccountCommandHandler : CommandHandlerBase<UpdateAccountCommand, Unit>
    {
        public UpdateAccountCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Billing)
                .SingleOrDefaultAsync(x => !x.IsDeleted && !x.IsTemplate && x.Id == command.Id, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.Id);

            Mapper.Map(command, account);

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            // Add audit log
            await Mediator.Publish(new UserActionNotification()
            {
                User = command.User,
                Accounts = new[] { account },
                Action = "PushAccountProperties",
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