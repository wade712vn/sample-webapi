using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Application.Services;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Library;
using FluentValidation;
using FluentValidation.Validators;

namespace AccountManager.Application.Accounts.Commands.UpdateInstanceSettings
{
    public class UpdateInstanceSettingsCommandValidator : AbstractValidator<UpdateInstanceSettingsCommand>
    {
        private readonly IAccountManagerDbContext _context;
        private readonly ILibraryFileService _libraryFileService;

        public UpdateInstanceSettingsCommandValidator(IAccountManagerDbContext context, ILibraryFileService libraryFileService)
        {
            _context = context;
            _libraryFileService = libraryFileService;

            RuleFor(x => x).CustomAsync(LibraryFileValid);
        }

        private async Task LibraryFileValid(UpdateInstanceSettingsCommand command, CustomContext context, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Class)
                .FirstOrDefaultAsync(x => x.Id == command.AccountId, cancellationToken);

            if (account == null)
            {
                context.AddFailure("Invalid account");
                return;
            }

            var mmaClass = account.Class;
            if (mmaClass == null)
            {
                context.AddFailure("Invalid MMA class");
            }


            var libraryFiles = await _context.Set<File>().Where(x => command.MainLibraryFiles.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var libraryFile in libraryFiles)
            {
                try
                {
                    if (!await _libraryFileService.FileExists(libraryFile.Url))
                    {
                        context.AddFailure($"Library file {libraryFile.Name} doesn't exist on S3");
                    }
                }
                catch (Exception e)
                {
                    // Ignore
                }

                if (mmaClass != null && mmaClass.IsProduction && libraryFile.ReleaseStage != ReleaseStage.Released)
                {
                    context.AddFailure($"Library file {libraryFile.Name} must not be used for production account(s)");
                }
            }

        }
    }
}
