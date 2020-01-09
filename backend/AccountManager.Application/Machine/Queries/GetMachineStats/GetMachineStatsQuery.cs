using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountManager.Application.Queries.GetMachineStats
{
    public class GetMachineStatsQuery : IRequest<MachineStatsDto>
    {
    }

    public class GetMachineStatsQueryHandler : IRequestHandler<GetMachineStatsQuery, MachineStatsDto>
    {
        private readonly IAccountManagerDbContext _context;

        public GetMachineStatsQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<MachineStatsDto> Handle(GetMachineStatsQuery request, CancellationToken cancellationToken)
        {
            var machines = await _context.Set<Machine>()
                .Include(x => x.CloudInstances)
                .Include(x => x.CloudInstanceType)
                .Include(x => x.Class)
                .ToListAsync(cancellationToken);

            var machineStatsDto = new MachineStatsDto
            {
            };

            var machinesByClass = new Dictionary<string, int>();

            foreach (var machine in machines)
            {
                var instance = machine.CloudInstances.FirstOrDefault();

                machineStatsDto.Total++;

                if (machine.Class != null)
                {
                    if (!machinesByClass.ContainsKey(machine.Class.Name)) {
                        machinesByClass.Add(machine.Class.Name, 0);
                    }

                    machinesByClass[machine.Class.Name]++;
                }

                if (machine.Managed.HasValue && machine.Managed.Value && machine.NeedsAdmin)
                    machineStatsDto.NeedsAdmin++;

                if (machine.Managed.HasValue && machine.Managed.Value)
                    machineStatsDto.Managed++;

                if (machine.Managed.HasValue && machine.Managed.Value && machine.Turbo.HasValue && machine.Turbo.Value)
                    machineStatsDto.Turbo++;

                var creditCost = machine.CloudInstanceType?.CloudCreditCost ?? 0;
                if (instance == null)
                {
                    continue;
                }
                else if (instance.Status == "running")
                {
                    machineStatsDto.RunningCost += creditCost;
                    machineStatsDto.TotalCost += creditCost;
                }
                else if (instance.Status == "stopped")
                {
                    machineStatsDto.TotalCost += creditCost;
                }
            }

            machineStatsDto.ClassStats = machinesByClass.Select(x => new
            {
                Class = x.Key,
                Total = x.Value
            });

            return machineStatsDto;
        }
    }
}
