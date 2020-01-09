using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.DeleteAccount
{
    public class DeleteAccountCommand : CommandBase
    {
        public long Id { get; set; }
    }
}
