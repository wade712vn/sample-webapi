using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate
{
    public class CreateOrUpdateInstanceSettingsTemplateCommandHandler : IRequestHandler<CreateOrUpdateInstanceSettingsTemplateCommand, long>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateInstanceSettingsTemplateCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(CreateOrUpdateInstanceSettingsTemplateCommand request, CancellationToken cancellationToken)
        {
            MachineConfig instanceSettingsTemplate;
            if (request.Id > 0)
            {
                instanceSettingsTemplate = await _context.Set<MachineConfig>()
                    .FirstOrDefaultAsync(x => x.IsTemplate && x.Id == request.Id, cancellationToken);

                if (instanceSettingsTemplate == null)
                    throw new EntityNotFoundException(nameof(MachineConfig), request.Id);
            }
            else
            {
                instanceSettingsTemplate = new MachineConfig() { IsTemplate = true };
                _context.Set<MachineConfig>().Add(instanceSettingsTemplate);
            }

            Mapper.Map(request, instanceSettingsTemplate);

            await _context.SaveChangesAsync(cancellationToken);

            return instanceSettingsTemplate.Id;
        }
    }
}