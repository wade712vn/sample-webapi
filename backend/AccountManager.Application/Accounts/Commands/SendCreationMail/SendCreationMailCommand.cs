using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManager.Application.Accounts.Commands.SendCreationMail
{
    public class SendCreationMailCommand : CommandBase
    {
        public long AccountId { get; set; }
    }
}
