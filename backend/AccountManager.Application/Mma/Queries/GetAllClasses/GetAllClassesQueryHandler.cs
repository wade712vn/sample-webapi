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

namespace AccountManager.Application.Queries.GetAllClasses
{
    public class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, IEnumerable<ClassDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllClassesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassDto>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
        {
            var classes = await _context.Set<Class>().Include(x => x.MmaInstances).ToListAsync(cancellationToken);
            return Mapper.Map<IEnumerable<ClassDto>>(classes);
        }
    }
}