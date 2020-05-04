using System;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    /// <summary>
    /// Extension for <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Method adds services into <see cref="IServiceCollection"/> by passed strategy implementation
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(
            this IServiceCollection collection, 
            IDependenciesRegistrationStrategy strategy)
        {
            void Action(Type source, Type to, ServiceLifetime lifetime) =>
                collection.Add(new ServiceDescriptor(source, to, lifetime));

            strategy.Register(Action);

            return collection;
        }
    }
}