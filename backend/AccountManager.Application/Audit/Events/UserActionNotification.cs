using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Audit.Events
{
    public class UserActionNotification : INotification
    {
        public string Action { get; set; }
        public string User { get; set; }
        public Account[] Accounts { get; set; }
        public Machine[] Machines { get; set; }
        public dynamic Data { get; set; }
    }
}
