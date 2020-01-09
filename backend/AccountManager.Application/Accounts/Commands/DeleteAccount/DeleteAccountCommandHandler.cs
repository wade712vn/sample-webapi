﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.DeleteAccount
{
    public class DeleteAccountCommandHandler : CommandHandlerBase<DeleteAccountCommand, Unit>
    {
        public DeleteAccountCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Where(x => x.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.Id);

            account.IsDeleted = true;

            // Unset desired state of machines

            var machines = Context.Set<Machine>()
                .Include(x => x.States)
                .Where(x => x.AccountId == account.Id);

            foreach (var machine in machines)
            {
                machine.Terminate = true;
            }

            await Context.SaveChangesAsync(cancellationToken);

            // Add audit log
            await Mediator.Publish(new UserActionNotification()
            {
                User = command.User,
                Accounts = new[] { account },
                Action = "DeleteAccount",
                Data = new
                {
                    Params = command
                }

            }, cancellationToken);

            return Unit.Value;
        }
    }
}