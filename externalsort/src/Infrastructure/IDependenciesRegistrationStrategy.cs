using System;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    /// <summary>
    /// Basic strategy for registering services in extensible way
    /// </summary>
    public interface IDependenciesRegistrationStrategy
    {
        /// <summary>
        /// Method take delegate to register interface to implementation with particular lifetime
        /// </summary>
        /// <param name="action">Registering action</param>
        void Register(Action<Type, Type, ServiceLifetime> action);
    }
}