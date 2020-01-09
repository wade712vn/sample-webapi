using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AccountManager.Application.Mappings;
using AccountManager.Persistence;
using Autofac;

namespace AccountManager.WebApi.Infrastructure.AutofacModules
{
    public class AutoMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OperationTypeValueResolver>().SingleInstance();
            builder.RegisterType<StringVersionInfoConverter>().SingleInstance();
        }
    }
}