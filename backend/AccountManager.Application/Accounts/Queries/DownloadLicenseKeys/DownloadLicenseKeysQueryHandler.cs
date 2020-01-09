using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Queries.DownloadLicenseKeys;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.DownloadLicenseKeys
{
    public class DownloadLicenseKeysQueryHandler : IRequestHandler<DownloadLicenseKeysQuery, DownloadableDto>
    {
        private readonly IAccountManagerDbContext _context;

        public DownloadLicenseKeysQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<DownloadableDto> Handle(DownloadLicenseKeysQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Keys)
                .Where(x => x.Id == request.AccountId)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountId);

            var licensePrivateKey = account.Keys.LicensePrivate;
            var licensePublicKey = account.Keys.LicensePublic;

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var publicKeyEntry = zipArchive.CreateEntry("LicenseKey.public");
                    using (var writer = new StreamWriter(publicKeyEntry.Open()))
                    {
                        writer.Write(licensePublicKey);
                    }

                    var privateKeyEntry = zipArchive.CreateEntry("LicenseKey.private");
                    using (var writer = new StreamWriter(privateKeyEntry.Open()))
                    {
                        writer.Write(licensePrivateKey);
                    }
                }

                var bytes = memoryStream.ToArray();
                return new DownloadableDto
                {
                    Content = bytes,
                    FileName = $"{account.UrlFriendlyName} license-keys.zip"
                };
            }
        }
    }
}