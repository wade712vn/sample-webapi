using System;
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

namespace AccountManager.Application.Cloud.Queries.GetAllInstanceTypes
{
    public class GetAllInstanceTypesQueryHandler : IRequestHandler<GetAllInstanceTypesQuery, List<CloudInstanceTypeDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllInstanceTypesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<CloudInstanceTypeDto>> Handle(GetAllInstanceTypesQuery request, CancellationToken cancellationToken)
        {
            var instanceTypes = await _context.Set<CloudInstanceType>().OrderBy(x => x.StorageSize).ToListAsync(cancellationToken);
            return await Task.FromResult(Mapper.Map<List<CloudInstanceTypeDto>>(instanceTypes));
        }
    }
}