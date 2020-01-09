using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Exceptions;
using AccountManager.Application.License;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateLicenseSettings
{
    public class UpdateLicenseSettingsCommandHandler : CommandHandlerBase<UpdateLicenseSettingsCommand, Unit>
    {
        public UpdateLicenseSettingsCommandHandler(IMediator mediator, IAccountManagerDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(UpdateLicenseSettingsCommand command, CancellationToken cancellationToken)
        {
            var account = Context.Set<Account>()
                .Include(x => x.Keys)
                .Include(x => x.LicenseConfig)
                .FirstOrDefault(x => !x.IsDeleted && x.Id == command.AccountId);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);

            var keys = account.Keys;
            var licenseConfig = account.LicenseConfig;

            Mapper.Map(command, licenseConfig);

            var licenseBytes = LicenseUtils.GenerateLicense(account.LicenseConfig, keys.LicensePrivate);
            account.License = Convert.ToBase64String(licenseBytes);

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            // Add audit log
            await Mediator.Publish(new UserActionNotification()
            {
                User = command.User,
                Accounts = new [] { account },
                Action = "PushLicenseSettings",
                Data = new
                {
                    Params = command,
                    Changes = changes
                }

            }, cancellationToken);

            return Unit.Value;
        }
    }
}
