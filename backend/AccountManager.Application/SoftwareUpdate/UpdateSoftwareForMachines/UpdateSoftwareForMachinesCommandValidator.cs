using AccountManager.Application.Common.Interfaces;
using FluentValidation;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class UpdateSoftwareForMachinesCommandValidator : AbstractValidator<UpdateSoftwareForMachinesCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public UpdateSoftwareForMachinesCommandValidator(IAccountManagerDbContext context)
        {
            _context = context;
        }
    }
}
