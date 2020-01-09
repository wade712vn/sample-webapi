using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Library.Commands.DeleteLibraryFile;
using AccountManager.Application.Common.Interfaces;
using MediatR;

namespace AccountManager.Application.Library.Commands.BatchDeleteLibraryFiles
{
    public class BatchDeleteLibraryFilesCommand : CommandBase
    {
        public long[] Ids { get; set; }
    }

    public class BatchDeleteLibraryFilesCommandHandler : IRequestHandler<BatchDeleteLibraryFilesCommand>
    {
        private readonly IAccountManagerDbContext _context;
        private readonly IMediator _mediator;

        public BatchDeleteLibraryFilesCommandHandler(IAccountManagerDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(BatchDeleteLibraryFilesCommand request, CancellationToken cancellationToken)
        {
            using (var transaction = _context.Database.BeginTransaction())
                try
                {
                    foreach (var id in request.Ids)
                    {
                        var deleteFileCommand = new DeleteLibraryFileCommand {Id = id};
                        await _mediator.Send(deleteFileCommand, cancellationToken);
                    }

                    transaction.Commit();
                    return Unit.Value;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
        }
    }
}
