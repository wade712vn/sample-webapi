using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAccount
{
    public class GetAccountQuery : IRequest<AccountDto>
    {
        public long Id { get; set; }
    }
}