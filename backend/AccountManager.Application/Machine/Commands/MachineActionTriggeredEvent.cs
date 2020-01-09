using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Commands
{
    public class MachineActionTriggeredEvent : INotification
    {
        public string User { get; set; }
        public Machine Machine { get; set; }
        public string Action { get; set; }
    }
}
