using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AccountManager.Application.Common.Interfaces;
using AccountManager.Persistence;
using Autofac;

namespace AccountManager.WebApi.Infrastructure.AutofacModules
{
    public class EfModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AccountManagerDb"].ConnectionString;

            builder.Register<AccountManagerDbContext>(x => new AccountManagerDbContext("AccountManagerDb")).As<IAccountManagerDbContext>()
                .InstancePerLifetimeScope();
        }
    }
}