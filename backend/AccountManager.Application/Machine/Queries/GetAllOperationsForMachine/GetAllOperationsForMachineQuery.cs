using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Queries.GetAllOperationsForMachine
{
    public class GetAllOperationsForMachineQuery : IRequest<IEnumerable<OperationDto>>
    {
        public long Id { get; set; }
    }
}