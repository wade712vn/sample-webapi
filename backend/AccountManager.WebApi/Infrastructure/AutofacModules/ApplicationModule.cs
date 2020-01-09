using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using AccountManager.Application;
using AccountManager.Application.Auth;
using AccountManager.Application.Caching;
using AccountManager.Application.Key;
using AccountManager.Application.Logging;
using AccountManager.Application.Services;
using AccountManager.Infrastructure.Auth;
using AccountManager.Infrastructure.Caching;
using AccountManager.Infrastructure.Logging;
using AccountManager.Infrastructure.S3;
using Autofac;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AccountManager.WebApi.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KeysManager>()
                .As<IKeysManager>()
                .SingleInstance();

            builder.RegisterType<TaskTrackingService>().As<ITaskTrackingService>().SingleInstance();
            builder.RegisterType<S3LibrafyFileService>().As<ILibraryFileService>()
                .WithParameter("config", new S3Config
                {
                    AwsAccessKey = ConfigurationManager.AppSettings["s3Library:aws:AccessKey"],
                    AwsSecretKey = ConfigurationManager.AppSettings["s3Library:aws:SecretKey"],
                    AwsRegion = ConfigurationManager.AppSettings["s3Library:aws:Region"],
                    AwsBucket = ConfigurationManager.AppSettings["s3Library:aws:Bucket"],
                })
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(CreateSiteTaskStatusUpdater).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(ITaskStatusUpdater<>));

            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().SingleInstance();

            builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance();
            builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance();
            builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
            builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance();

            var signingKey = ConfigurationManager.AppSettings["jwt:SigningKey"];
            var issuer = ConfigurationManager.AppSettings["jwt:Issuer"];
            var audience = ConfigurationManager.AppSettings["jwt:Audience"];

            builder.RegisterInstance(new JwtIssuerOptions()
            {
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)), SecurityAlgorithms.HmacSha256)
            }).As<JwtIssuerOptions>();

            builder.RegisterType<Logger>().As<ILogger>();

            builder.RegisterType<SoftwareVersionResolver>()
                .As<ISoftwareVersionResolver>()
                .InstancePerLifetimeScope();
        }
    }
}