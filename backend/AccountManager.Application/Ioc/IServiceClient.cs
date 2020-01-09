using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Application.Ioc
{
    public interface IServiceClient
    {
        T Resolve<T>() where T : class;

        object Resolve(Type type);
    }
}
