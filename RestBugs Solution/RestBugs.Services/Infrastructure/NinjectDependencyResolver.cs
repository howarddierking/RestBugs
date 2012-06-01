using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Dependencies;
using Ninject;

namespace RestBugs.Services.Infrastructure
{
    class NinjectDependencyResolver : IDependencyResolver
    {
        private KernelBase _kernel;

        public NinjectDependencyResolver(KernelBase kernel)
        {
            _kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
