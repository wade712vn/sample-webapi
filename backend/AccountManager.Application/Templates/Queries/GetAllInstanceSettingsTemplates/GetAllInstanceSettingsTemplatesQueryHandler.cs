using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Library;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllInstanceSettingsTemplates
{
    public class GetAllInstanceSettingsTemplatesQueryHandler : IRequestHandler<GetAllInstanceSettingsTemplatesQuery, IEnumerable<MachineConfigDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllInstanceSettingsTemplatesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MachineConfigDto>> Handle(GetAllInstanceSettingsTemplatesQuery request, CancellationToken cancellationToken)
        {
            var licenseTemplates = await _context.Set<MachineConfig>()
                .Include(x => x.Account)
                .Where(x => x.Account == null || (x.Account != null && !x.Account.IsDeleted))
                .ToListAsync(cancellationToken);

            var allLibraryFileIds = licenseTemplates.SelectMany(x => x.MainLibraryFiles.DeserializeArray<long>())
                .Union(licenseTemplates.Where(x => x.AccountLibraryFile.HasValue).Select(x => x.AccountLibraryFile.Value)).Distinct();

            var libraryFiles = await _context.Set<File>()
                .Where(x => x.Id != 0 && allLibraryFileIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var libraryFileMap = Mapper.Map<IEnumerable<FileDto>>(libraryFiles).ToDictionary(x => x.Id, x => x);

            var licenseTemplateDtos = Mapper.Map<List<MachineConfigDto>>(licenseTemplates);

            foreach (var licenseTemplateDto in licenseTemplateDtos)
            {
                licenseTemplateDto.MainLibraryFiles = licenseTemplateDto.MainLibraryFileIds.Select(x =>
                {
                    FileDto mainLibraryFile;
                    return libraryFileMap.TryGetValue(x, out mainLibraryFile) ? mainLibraryFile : null;

                }).Where(x => x != null).ToArray();

                if (licenseTemplateDto.AccountLibraryFileId.HasValue)
                {
                    FileDto accountLibraryFile;
                    licenseTemplateDto.AccountLibraryFile =
                        libraryFileMap.TryGetValue(licenseTemplateDto.AccountLibraryFileId.Value, out accountLibraryFile)
                            ? accountLibraryFile
                            : null;
                }

            }

            return licenseTemplateDtos;
        }

    }
}