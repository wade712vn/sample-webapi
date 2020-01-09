using System;
using AccountManager.Domain.Entities;

namespace AccountManager.Application.Models.Dto
{
    public class OperationDto
    {
        public long Id { get; set; }
        public long MachineId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int FailCounter { get; set; }
        public string Params { get; set; }
        public int FallbackLevel { get; set; }
        public OperationTypeDto Type { get; set; }
        public string TypeName { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? FinishTime { get; set; }
        public string Output { get; set; }

        public MachineDto MachineDto { get; set; }
    }
}