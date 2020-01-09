using System.Linq;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Domain.Entities;
using FluentValidation;
using FluentValidation.Validators;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate
{
    public class CreateOrUpdateInstanceSettingsTemplateCommandValidator : AbstractValidator<CreateOrUpdateInstanceSettingsTemplateCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateInstanceSettingsTemplateCommandValidator(IAccountManagerDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Template name is required")
                .Must(NameUnique).WithMessage("Template name must be unique").When(x => x.Id > 0);
        }

        private bool NameUnique(CreateOrUpdateInstanceSettingsTemplateCommand command, string name, PropertyValidatorContext context)
        {
            return !_context.Set<MachineConfig>().Any(x => x.Id != command.Id && x.Name == name);
        }
    }
}
