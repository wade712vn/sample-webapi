using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Git.Queries.GetAllRepos;
using AccountManager.Application.Key;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AccountManager.Application.Library.Queries.GetAllLibraryPackages
{
    public class GetAllLibraryPackagesQueryHandler : IRequestHandler<GetAllLibraryPackagesQuery, List<PackageDto>>
    {
        private readonly IAccountManagerDbContext _context;

        public GetAllLibraryPackagesQueryHandler(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public async Task<List<PackageDto>> Handle(GetAllLibraryPackagesQuery request, CancellationToken cancellationToken)
        {
            var packages = await _context.Set<Package>().ToListAsync(cancellationToken);

            var dtos = Mapper.Map<List<PackageDto>>(packages);

            return dtos;
        }
    }
}
