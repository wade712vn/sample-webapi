﻿using System;
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

namespace AccountManager.Application.Saas.Commands.DeleteSite
{
    public class DeleteSiteCommand : CommandBase
    {
        public string UrlFriendlyName { get; set; }
        public string AccountUrlFriendlyName { get; set; }
    }

    public class DeleteSiteCommandHandler : IRequestHandler<DeleteSiteCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public DeleteSiteCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UrlFriendlyName == request.AccountUrlFriendlyName, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountUrlFriendlyName);

            var site = await _context.Set<Site>()
                .Include(x => x.Machine)
                .FirstOrDefaultAsync(x => x.UrlFriendlyName == request.UrlFriendlyName, cancellationToken);

            if (site == null)
                throw new EntityNotFoundException(nameof(Site), request.UrlFriendlyName);

            var machine = site.Machine;
            machine.Terminate = true;

            _context.Set<Site>().Remove(site);

            await _context.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(Unit.Value);
        }
    }
}
