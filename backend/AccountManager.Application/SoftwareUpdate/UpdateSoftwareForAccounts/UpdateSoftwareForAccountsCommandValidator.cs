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

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts
{
    public class UpdateSoftwareForAccountsCommandValidator : AbstractValidator<UpdateSoftwareForAccountsCommand>
    {
        private readonly IAccountManagerDbContext _context;
        private readonly ILibraryFileService _libraryFileService;

        public UpdateSoftwareForAccountsCommandValidator(IAccountManagerDbContext context, ILibraryFileService libraryFileService)
        {
            _context = context;
            _libraryFileService = libraryFileService;

            RuleFor(x => x).CustomAsync(LibraryFileValid);
        }

        private async Task LibraryFileValid(UpdateSoftwareForAccountsCommand command, CustomContext context, CancellationToken cancellationToken)
        {
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
            }

        }
    }
}
