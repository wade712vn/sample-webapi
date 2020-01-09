using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Ioc;
using Autofac;

namespace AccountManager.Infrastructure.Ioc
{
    public class ServiceClient : IServiceClient
    {
        private readonly IComponentContext _container;

        public ServiceClient(IComponentContext container)
        {
            _container = container;
        }

        public T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }

    
}
