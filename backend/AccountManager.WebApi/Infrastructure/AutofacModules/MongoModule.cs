using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AccountManager.Domain.Repositories;
using AccountManager.Persistence;
using AccountManager.Persistence.Repositories;
using Autofac;

namespace AccountManager.WebApi.Infrastructure.AutofacModules
{
    public class MongoModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AccountManagerMongoDb"].ConnectionString;
            var databaseName = "AMv2";

            builder.Register(c =>
            {
                var context = new AMMongoContext(new MongoContextSettings()
                {
                    ConnectionString = connectionString,
                    Database = databaseName
                });
                return context;
            }).As<AMMongoContext>().InstancePerLifetimeScope();

            builder.RegisterType<AuditLogRepository>().As<IAuditLogRepository>().InstancePerLifetimeScope();
        }
    }
}