using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AccountManager.Application.Ioc;
using AccountManager.Application.Mappings;
using AccountManager.Application.Services;
using AccountManager.Common;
using AccountManager.Infrastructure.Ioc;
using AccountManager.Persistence;
using AccountManager.WebApi.Controllers.SaasApi;
using AccountManager.WebApi.Filters;
using AccountManager.WebApi.Infrastructure.AutofacModules;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AccountManager.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            SerializationConfig.RegisterSerializationSetting();

            var config = GlobalConfiguration.Configuration;

            Database.SetInitializer<AccountManagerDbContext>(null);

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyModules(typeof(EfModule).Assembly);
            builder.RegisterType<ServiceClient>().As<IServiceClient>().SingleInstance();
            builder.RegisterWebApiFilterProvider(config);
            builder.Register(c => new SaasBasicAuthenticationFilter(c.Resolve<IMediator>()))
                .AsWebApiAuthenticationFilterFor<SaasApiControllerBase>().InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            AutoMapperConfig.RegisterProfiles(container);

            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy =
                IncludeErrorDetailPolicy.Always;

            var taskTrackingService = container.Resolve<ITaskTrackingService>();
            taskTrackingService.Init();
        }
    }

    public static class AutoMapperConfig
    {
        public static void RegisterProfiles(IContainer container)
        {
            Mapper.Initialize(m =>
            {
                m.ConstructServicesUsing(container.Resolve);
                m.AddProfiles(typeof(DtoMappingProfile).Assembly);
            });
        }
    }

    public static class SerializationConfig
    {
        public static void RegisterSerializationSetting()
        {
            var config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
                (new Newtonsoft.Json.Converters.StringEnumConverter());

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
                (new PatchConverter());

        }
    }
}
