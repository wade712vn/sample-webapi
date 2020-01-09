using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateLicenseTemplate
{
    public class CreateOrUpdateLicenseTemplateCommandHandler : IRequestHandler<CreateOrUpdateLicenseTemplateCommand, long>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateLicenseTemplateCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateOrUpdateLicenseTemplateCommand request, CancellationToken cancellationToken)
        {
            LicenseConfig licenseTemplate;
            if (request.Id > 0)
            {
                licenseTemplate = await _context.Set<LicenseConfig>()
                    .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == request.Id, cancellationToken);

                if (licenseTemplate == null)
                    throw new EntityNotFoundException(nameof(LicenseConfig), request.Id);
            }
            else
            {
                licenseTemplate = new LicenseConfig() { IsTemplate = true };
                _context.Set<LicenseConfig>().Add(licenseTemplate);
            }

            Mapper.Map(request, licenseTemplate);
            await _context.SaveChangesAsync(cancellationToken);

            return licenseTemplate.Id;
        }
    }
}