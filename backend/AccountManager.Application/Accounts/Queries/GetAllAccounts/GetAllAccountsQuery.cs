using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsQuery : IRequest<List<AccountListingDto>>
    {
    }
}