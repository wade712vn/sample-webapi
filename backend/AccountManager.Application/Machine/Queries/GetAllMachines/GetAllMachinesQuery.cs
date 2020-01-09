﻿using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Queries.GetAllMachines
{
    public class GetAllMachinesQuery : IRequest<IEnumerable<MachineDto>>
    {
        public bool IncludeDeletedAccounts { get; set; }
    }
}