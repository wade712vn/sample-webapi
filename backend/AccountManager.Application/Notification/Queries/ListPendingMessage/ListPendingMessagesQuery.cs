using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Notification.Queries.ListPendingMessage
{
    public class ListPendingMessagesQuery : IRequest<IEnumerable<Message>>
    {
    }

    public class ListPendingMessagesQueryHandler : IRequestHandler<ListPendingMessagesQuery, IEnumerable<Message>>
    {
        private readonly IAccountManagerDbContext _context;

        public ListPendingMessagesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> Handle(ListPendingMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Set<Message>()
                .Include(x => x.Machine)
                .OrderBy(x => x.Timestamp).ToListAsync(cancellationToken);
        }
    }
}
