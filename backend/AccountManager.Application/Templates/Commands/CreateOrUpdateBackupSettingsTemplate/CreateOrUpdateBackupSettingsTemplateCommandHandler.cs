using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate
{
    public class CreateOrUpdateBackupSettingsTemplateCommandHandler : IRequestHandler<CreateOrUpdateBackupSettingsTemplateCommand, long>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateBackupSettingsTemplateCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateOrUpdateBackupSettingsTemplateCommand request, CancellationToken cancellationToken)
        {
            BackupConfig backupSettingsTemplate;
            if (request.Id > 0)
            {
                backupSettingsTemplate = await _context.Set<BackupConfig>()
                    .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == request.Id, cancellationToken);

                if (backupSettingsTemplate == null)
                    throw new EntityNotFoundException(nameof(BackupConfig), request.Id);
            }
            else
            {
                backupSettingsTemplate = new BackupConfig() { IsTemplate = true };
                _context.Set<BackupConfig>().Add(backupSettingsTemplate);
            }

            Mapper.Map(request, backupSettingsTemplate);

            await _context.SaveChangesAsync(cancellationToken);

            return backupSettingsTemplate.Id;
        }
    }
}