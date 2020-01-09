using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities.Library;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Library.Commands.DeleteLibraryFile
{
    public class DeleteLibraryFileCommand : CommandBase
    {
        public long Id { get; set; }
    }

    public class DeleteLibraryFileCommandHandler : IRequestHandler<DeleteLibraryFileCommand>
    {
        private readonly IAccountManagerDbContext _context;

        public DeleteLibraryFileCommandHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteLibraryFileCommand command, CancellationToken cancellationToken)
        {
            var file = await _context.Set<File>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (file == null)
                throw new EntityNotFoundException(nameof(File), command.Id);

            _context.Set<File>().Remove(file);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
