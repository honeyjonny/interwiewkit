using System;
using System.Threading;
using Infrastructure;
using Infrastructure.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    /// <summary>
    /// Class represents most common infrastructure for testing purposes
    /// </summary>
    public class TestBase
    {
        private readonly Lazy<IServiceProvider> _serviceProviderLazy =
            new Lazy<IServiceProvider>(CreateProvider, LazyThreadSafetyMode.ExecutionAndPublication);

        public IServiceProvider ServiceProvider => _serviceProviderLazy.Value;

        private static IServiceProvider CreateProvider()
        {
            (_, IServiceCollection collection) = ServicesInfrastructureFactory.Get();

            collection.AddServices(new TestsServicesInfrastructureStrategy());

            return collection.BuildServiceProvider();
        }
    }
}