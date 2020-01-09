using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Accounts.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<IEnumerable<string>>
    {
    }

    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<string>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllCustomersQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _context.Set<Account>().Where(x => x.Customer != null && x.Customer.Trim() != string.Empty).Distinct().Select(x => x.Customer).ToListAsync(cancellationToken);

            return customers;
        }
    }
}
