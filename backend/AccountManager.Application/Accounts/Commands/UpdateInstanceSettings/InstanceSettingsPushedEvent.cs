using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateInstanceSettings
{
    public class InstanceSettingsPushedEvent : INotification
    {
        public string Actor { get; internal set; }
        public Account Account { get; internal set; }
        public dynamic Command { get; internal set; }
        public IEnumerable<EntityChangeLog> Changes { get; internal set; }
    }
}
