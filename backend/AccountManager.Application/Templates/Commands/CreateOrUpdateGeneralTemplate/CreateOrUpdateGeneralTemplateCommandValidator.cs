using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Domain.Entities;
using FluentValidation;
using FluentValidation.Validators;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateGeneralTemplate
{
    public class CreateOrUpdateGeneralTemplateCommandValidator : AbstractValidator<CreateOrUpdateGeneralTemplateCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateGeneralTemplateCommandValidator(IAccountManagerDbContext context)
        {
            _context = context;

            RuleFor(x => x).CustomAsync(TemplatesNamesUnique);
        }

        private async Task TemplatesNamesUnique(CreateOrUpdateGeneralTemplateCommand command, CustomContext context, CancellationToken cancellationToken)
        {
            if (_context.Set<Account>().Any(x => x.Id != command.Id && x.Name == command.Name))
            {
                context.AddFailure($"General template {command.Name} already exists");
            }

            if (_context.Set<LicenseConfig>().Any(x => (x.Account == null || x.Account.Id != command.Id) && x.Name == command.Name))
            {
                context.AddFailure($"License template {command.Name} already exists");
            }

            if (_context.Set<MachineConfig>().Any(x => (x.Account == null || x.Account.Id != command.Id) && x.Name == command.Name))
            {
                context.AddFailure($"Instance settings template {command.Name} already exists");
            }

            if (_context.Set<BackupConfig>().Any(x => (x.Account == null || x.Account.Id != command.Id) && x.Name == command.Name))
            {
                context.AddFailure($"Backup settings template {command.Name} already exists");
            }
        }
    }
}