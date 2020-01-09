using AccountManager.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class SoftwareUpdatedEvent : INotification
    {
        private IEnumerable<Machine> _machines;
        private IEnumerable<Account> _accounts;
        public string Actor { get; internal set; }

        public IEnumerable<Machine> Machines
        {
            get => _machines ?? (_machines = new List<Machine>());
            internal set => _machines = value;
        }

        public IEnumerable<Account> Accounts
        {
            get => _accounts ?? (_accounts = new List<Account>());
            internal set => _accounts = value;
        }

        public dynamic Command { get; internal set; }
    }
}
