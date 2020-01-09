using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Cloud.Queries.GetAllRegions
{
    public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, List<RegionDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllRegionsQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<RegionDto>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _context.Set<CloudRegion>().ToListAsync(cancellationToken);
            return await Task.FromResult(Mapper.Map<List<RegionDto>>(regions));
        }
    }
}