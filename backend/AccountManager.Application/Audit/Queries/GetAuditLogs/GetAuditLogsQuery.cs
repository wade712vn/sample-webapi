using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Audit;
using AccountManager.Domain.Repositories;
using AccountManager.Application.Common.Interfaces;
using MediatR;
using SortDirection = AccountManager.Domain.SortDirection;

namespace AccountManager.Application.Audit.Queries.GetAuditLogs
{
    public class GetAuditLogsQuery : IRequest<IEnumerable<AuditLog>>
    {
        public long? AccountId { get; set; }
        public string Action { get; set; }
        public int StartIndex { get; set; }
        public int Limit { get; set; }
        public SortDirection SortDirection { get; set; }
    }

    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, IEnumerable<AuditLog>>
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IEnumerable<AuditLog>> Handle(GetAuditLogsQuery query, CancellationToken cancellationToken)
        {
            return await _auditLogRepository.FindPagedAsync(query.AccountId, query.Action, query.StartIndex, query.Limit,
                query.SortDirection, cancellationToken);
        }
    }
}
