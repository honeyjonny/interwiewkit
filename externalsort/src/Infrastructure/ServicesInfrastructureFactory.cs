using System;
using Infrastructure.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    /// <summary>
    /// Class represents common infrastructure setup logic
    /// </summary>
    public static class ServicesInfrastructureFactory
    {
        public static (IServiceProvider provider, IServiceCollection currentCollection) Get()
        {
            ServiceCollection collection = new ServiceCollection();

            collection.AddLogging(builder => { builder.AddConsole(); });

            collection.AddServices(new DependenciesRegistrationStrategy());

            IServiceProvider provider = collection.BuildServiceProvider();
            return (provider, collection);
        }
    }
}