using System.Linq;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Domain.Entities;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateLicenseTemplate
{
    public class CreateOrUpdateLicenseTemplateCommandValidator : AbstractValidator<CreateOrUpdateLicenseTemplateCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public CreateOrUpdateLicenseTemplateCommandValidator(IAccountManagerDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Template name is required")
                .Must(NameUnique).WithMessage("Template name must be unique").When(x => x.Id > 0);

            
        }

        private bool NameUnique(CreateOrUpdateLicenseTemplateCommand command, string name, PropertyValidatorContext context)
        {
            return !_context.Set<LicenseConfig>().Any(x => x.Id != command.Id && x.Name == name);
        }
    }
}