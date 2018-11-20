using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Dfc.Coursedata.Enrichment.Common.DependencyInjection
{
    internal class InjectBindingProvider : IInjectBindingProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public InjectBindingProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context) =>
            Task.FromResult((IBinding)new InjectBinding(_serviceProvider, context.Parameter.ParameterType));
    }
}