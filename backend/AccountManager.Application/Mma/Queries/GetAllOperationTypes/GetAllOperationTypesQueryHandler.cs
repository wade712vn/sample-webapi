using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Mma.Queries.GetAllClasses;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Mma.Queries.GetAllOperationTypes
{
    public class GetAllOperationTypesQueryHandler : IRequestHandler<GetAllOperationTypesQuery, IEnumerable<OperationTypeDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllOperationTypesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OperationTypeDto>> Handle(GetAllOperationTypesQuery request, CancellationToken cancellationToken)
        {
            var classes = await _context.Set<OperationType>().ToListAsync(cancellationToken);
            return Mapper.Map<IEnumerable<OperationTypeDto>>(classes);
        }
    }
}