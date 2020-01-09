using System.Collections.Generic;
using System.Linq;
using AccountManager.Application.Caching;
using AccountManager.Application.Ioc;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;

namespace AccountManager.Application.Mappings
{
    public class OperationTypeValueResolver : IValueResolver<Operation, OperationDto, OperationTypeDto>
    {
        private readonly IServiceClient _serviceClient;

        public OperationTypeValueResolver(IServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public OperationTypeDto Resolve(Operation source, OperationDto destination, OperationTypeDto destMember,
            ResolutionContext context)
        {
            var dbContext = _serviceClient.Resolve<IAccountManagerDbContext>();
            var cacheManager = _serviceClient.Resolve<ICacheManager>();

            var operationTypes = cacheManager.Get<IEnumerable<OperationType>>("OperationTypes", () => dbContext.Set<OperationType>().ToList());
            var operationTypeMapByName = operationTypes.ToDictionary(x => x.Name, x => x);

            OperationType type;

            return operationTypeMapByName.TryGetValue(source.TypeName, out type) ? Mapper.Map<OperationTypeDto>(type) : new OperationTypeDto();
        }
    }
}