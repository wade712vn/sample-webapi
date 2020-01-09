using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.DownloadLicenseFile
{
    public class DownloadLicenseFileQueryHandler : IRequestHandler<DownloadLicenseFileQuery, DownloadableDto>
    {
        private readonly IAccountManagerDbContext _context;

        public DownloadLicenseFileQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<DownloadableDto> Handle(DownloadLicenseFileQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Where(x => x.Id == request.AccountId)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountId);

            var licenseFileStr = account.License;

            var bytes = System.Convert.FromBase64String(licenseFileStr);

            return new DownloadableDto
            {
                Content = bytes,
                FileName = $"{account.UrlFriendlyName} license.dat"
            };
        }
    }
}