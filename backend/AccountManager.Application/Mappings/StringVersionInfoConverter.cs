using System;
using System.Data.Entity;
using System.Linq;
using AccountManager.Application.Ioc;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Entities.Git;
using AccountManager.Application.Common.Interfaces;
using AutoMapper;

namespace AccountManager.Application.Mappings
{
    public class StringVersionInfoConverter : ITypeConverter<string, VersionInfo>
    {
        private readonly IServiceClient _serviceClient;

        public StringVersionInfoConverter(IServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public VersionInfo Convert(string source, VersionInfo destination, ResolutionContext context)
        {
            return new VersionInfo(source);
        }
    }
}