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

namespace AccountManager.Application.Cloud.Queries.GetAllImages
{
    public class GetAllImagesQueryHandler : IRequestHandler<GetAllImagesQuery, List<CloudBaseImageDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllImagesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<CloudBaseImageDto>> Handle(GetAllImagesQuery request, CancellationToken cancellationToken)
        {
            var instanceTypes = await _context.Set<CloudBaseImage>().OrderBy(x => x.Name).ToListAsync(cancellationToken);
            return await Task.FromResult(Mapper.Map<List<CloudBaseImageDto>>(instanceTypes));
        }
    }
}