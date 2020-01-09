using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application
{
    public class MachineUserActionEvent : INotification
    {
        public Machine Machine { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
    }
}
