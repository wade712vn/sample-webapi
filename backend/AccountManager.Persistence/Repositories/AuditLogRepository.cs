using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Audit;
using AccountManager.Domain.Repositories;
using MongoDB.Driver;
using SortDirection = AccountManager.Domain.SortDirection;

namespace AccountManager.Persistence.Repositories
{
    public class AuditLogRepository : MongoRepositoryBase<AuditLog>, IAuditLogRepository
    {
        
        public AuditLogRepository(AMMongoContext context) : base(context)
        {

        }

        protected override string CollectionName => "AuditLogs";

        public async Task<IEnumerable<AuditLog>> FindPagedAsync(long? accountId = null, string action = null, int startIndex = 0, int limit = 10,
            Domain.SortDirection sortDirection = Domain.SortDirection.Ascending,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var filter = Builders<AuditLog>.Filter.Empty;

            if (accountId.HasValue)
            {
                filter = filter & Builders<AuditLog>.Filter.ElemMatch(x => x.Accounts, x => x.Id == accountId);
            }

            if (!action.IsNullOrWhiteSpace())
            {
                filter = filter & Builders<AuditLog>.Filter.Eq(x => x.Action, action);
            }


            SortDefinition<AuditLog> sort;
            if (sortDirection == SortDirection.Ascending)
            {
                sort = Builders<AuditLog>.Sort.Ascending(x => x.Time);
            }
            else
            {
                sort = Builders<AuditLog>.Sort.Descending(x => x.Time);
            }

            var options = new FindOptions<AuditLog>
            {
                Sort = sort,
                Limit = limit,
                Skip = startIndex,
            };

            var cursor = await Context.AuditLogs.FindAsync(filter, options, cancellationToken);

            return await cursor.ToListAsync(cancellationToken);
        }
    }
}
