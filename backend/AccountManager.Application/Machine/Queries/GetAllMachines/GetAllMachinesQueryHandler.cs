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

namespace AccountManager.Application.Queries.GetAllMachines
{
    public class GetAllMachinesQueryHandler : IRequestHandler<GetAllMachinesQuery, IEnumerable<MachineDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllMachinesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MachineDto>> Handle(GetAllMachinesQuery request, CancellationToken cancellationToken)
        {
            var machines = _context.Set<Domain.Entities.Machine>()
                .Include(x => x.Account)
                .Where(x => !x.Account.IsDeleted)
                .Select(x => new
                {
                    x,
                    CloudInstances = x.CloudInstances.Where(y => y.Active),
                    Account = x.Account,
                    Class = x.Class,
                    Operations = x.Operations.Where(y => y.Active)
                })
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
