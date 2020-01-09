using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.UpdateIdleSchedule
{
    public class UpdateIdleScheduleCommand : CommandBase
    {
        public long AccountId { get; set; }

        public IEnumerable<IdleScheduleDto> IdleSchedules { get; set; }

    }
}
