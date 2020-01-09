using System;
using System.Data.Entity;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using MediatR;
using Newtonsoft.Json;

namespace AccountManager.Application.Queries.GetSiteServerStatusForMachine
{
    public class GetSiteServerStatusForMachineQuery : IRequest<SiteServerStatusDto>
    {
        public long Id { get; set; }
    }

    public class GetSiteServerStatusForMachineQueryHandler : IRequestHandler<GetSiteServerStatusForMachineQuery, SiteServerStatusDto>
    {
        private readonly IAccountManagerDbContext _context;

        public GetSiteServerStatusForMachineQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<SiteServerStatusDto> Handle(GetSiteServerStatusForMachineQuery query, CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Machine>()
                .Include(x => x.Account)
                .Include("Account.MachineConfig")
                .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), query.Id);

            var scheme = machine.Account.MachineConfig.EnableSsl ? "https" : "http";
            var baseDomainUrl = "";
            using (var httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"{scheme}://{machine.SiteName}.{machine.Account.UrlFriendlyName}.{baseDomainUrl}:8080")
            })
            {
                var response = await httpClient.GetAsync("irm/rest/v1.14/server-status", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new CommandException();
                }

                var serverStatus = JsonConvert.DeserializeObject<SiteServerStatusDto>(await response.Content.ReadAsStringAsync());

                return serverStatus;
            }
        }
    }

    public class SiteServerStatusDto
    {
        public string Status { get; set; }
        public bool Operational { get; set; }
        public bool MongoOk { get; set; }
        public bool InterServerCommOk { get; set; }
        public bool RabbitMqOk { get; set; }
        public bool LibraryImportsOk { get; set; }
        public bool DiskSpaceOk { get; set; }
    }
}
