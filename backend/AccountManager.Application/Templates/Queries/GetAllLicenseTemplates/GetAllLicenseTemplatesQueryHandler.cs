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

namespace AccountManager.Application.Templates.Queries.GetAllLicenseTemplates
{
    public class GetAllLicenseTemplatesQueryHandler : IRequestHandler<GetAllLicenseTemplatesQuery, IEnumerable<LicenseConfigDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllLicenseTemplatesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LicenseConfigDto>> Handle(GetAllLicenseTemplatesQuery request, CancellationToken cancellationToken)
        {
            var licenseTemplates = await _context.Set<LicenseConfig>()
                .Include(x => x.Account)
                .Where(x => x.Account == null || !x.Account.IsDeleted)
                .ToListAsync(cancellationToken);

            return Mapper.Map<List<LicenseConfigDto>>(licenseTemplates);
        }
    }
}