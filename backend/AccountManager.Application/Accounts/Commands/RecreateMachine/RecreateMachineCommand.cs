using System;
using System.Linq;
using System.Text;
using AccountManager.Domain.Constants;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.RecreateMachine
{
    public class RecreateMachineCommand : CommandBase
    {
        public long MachineId { get; set; }
    }
}
