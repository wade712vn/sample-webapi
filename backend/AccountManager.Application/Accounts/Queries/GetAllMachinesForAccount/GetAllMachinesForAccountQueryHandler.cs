using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllMachinesForAccount
{
    public class GetAllMachinesForAccountQueryHandler : IRequestHandler<GetAllMachinesForAccountQuery, IEnumerable<MachineDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllMachinesForAccountQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MachineDto>> Handle(GetAllMachinesForAccountQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Set<Machine>()
                .Include(x => x.Class)
                .Include(x => x.CloudInstanceType)
                .Where(x => (request.Id.HasValue && x.AccountId.HasValue && x.AccountId == request.Id) || x.Account.UrlFriendlyName == request.UrlFriendlyName)
                .Select(x => new
                {
                    x,
                    Class = x.Class,
                    CloudInstances = x.CloudInstances.Where(y => y.Active),
                    CloudInstanceType = x.CloudInstanceType,
                    Operations = x.Operations.Where(y => y.Active),
                    OperationTypes = x.Operations.Select(y => y.Type)
                });


            var machines = query
                .AsEnumerable()
                .Select(x => x.x)
                .ToList();

            var machineIds = machines.Select(x => x.Id).ToArray();
            var states = await _context.Set<State>().Where(x => x.Desired && x.MachineId.HasValue && machineIds.Contains(x.MachineId.Value)).ToListAsync(cancellationToken);

            machines.ForEach(machine =>
            {
                machine.States = states.Where(x => x.MachineId == machine.Id).ToList();
            });

            var instanceSettingsTemplates = await _context.Set<MachineConfig>().Where(x => x.IsTemplate).OrderByDescending(x => x.Id).ToListAsync(cancellationToken);

            var machineDtos = new List<MachineDto>();
            foreach (var machine in machines)
            {
                var machineDto = Mapper.Map<MachineDto>(machine);
                var matched = instanceSettingsTemplates.FirstOrDefault(x => MatchTemplate(x, machine));
                machineDto.BundleVersion = matched == null ? "Custom" : matched.Name;
                machineDtos.Add(machineDto);
            }

            return machineDtos;
        }

        private static bool MatchTemplate(MachineConfig config, Machine machine)
        {
            var state = machine.States.FirstOrDefault();
            if (state == null)
                return false;


            return (!machine.IsLauncher ||
                    new VersionInfo(config.LauncherHash).Hash == state.Launcher &&
                    new VersionInfo(config.PdfExportHash).Hash == state.PdfExport &&
                    new VersionInfo(config.ReportingHash).Hash == state.Reporting) &&
                   (!machine.IsSiteMaster ||
                    new VersionInfo(config.SiteMasterHash).Hash == state.SiteMaster &&
                    new VersionInfo(config.ClientHash).Hash == state.Client &&
                    new VersionInfo(config.RelExportHash).Hash == state.RelExport) &&
                   new VersionInfo(config.DeployerHash).Hash == state.Deployer &&
                   new VersionInfo(config.PopulateHash).Hash == state.Populate;
        }
    }
}
