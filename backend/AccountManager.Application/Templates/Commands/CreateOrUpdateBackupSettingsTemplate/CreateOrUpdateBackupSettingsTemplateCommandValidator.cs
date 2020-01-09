using System.Linq;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Domain.Entities;
using FluentValidation;
using FluentValidation.Validators;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate
{
    public class CreateOrUpdateBackupSettingsTemplateCommandValidator : AbstractValidator<CreateOrUpdateBackupSettingsTemplateCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateBackupSettingsTemplateCommandValidator(IAccountManagerDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Template name is required")
                .Must(NameUnique).WithMessage("Template name must be unique").When(x => x.Id > 0);
        }

        private bool NameUnique(CreateOrUpdateBackupSettingsTemplateCommand command, string name, PropertyValidatorContext context)
        {
            return !_context.Set<BackupConfig>().Any(x => x.Id != command.Id && x.Name == name);
        }
    }
}
