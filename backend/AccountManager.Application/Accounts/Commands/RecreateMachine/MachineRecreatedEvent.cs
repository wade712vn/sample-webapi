using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.RecreateMachine
{
    public class MachineRecreatedEvent : INotification
    {
        public string Actor { get; set; }
        public Machine Machine { get; set; }
        public dynamic Command { get; set; }
    }
}
