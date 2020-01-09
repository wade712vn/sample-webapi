using AccountManager.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Accounts.Commands.CreateAccount
{
    public class AccountCreatedEvent : INotification
    {
        public string User { get; internal set; }
        public Account Account { get; internal set; }
        public dynamic Command { get; set; }
    }
}
