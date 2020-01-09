using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Queries.GetAllOperationsForMachine
{
    public class GetAllOperationsForMachineQueryHandler : IRequestHandler<GetAllOperationsForMachineQuery, IEnumerable<OperationDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllOperationsForMachineQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OperationDto>> Handle(GetAllOperationsForMachineQuery request, CancellationToken cancellationToken)
        {
            var operations = await _context.Set<Operation>()
                .Include(x => x.Type)
                .Where(x => x.MachineId == request.Id)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync(cancellationToken);
            
            return Mapper.Map<IEnumerable<OperationDto>>(operations);
        }
    }
}
