using System;
using Abstractions;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Implementation
{
    /// <summary>
    /// Class represents particular strategy for registering dependencies
    /// </summary>
    public class DependenciesRegistrationStrategy: IDependenciesRegistrationStrategy
    {
        public void Register(Action<Type, Type, ServiceLifetime> action)
        {
            action(typeof(IDataProvider), typeof(TextFileDataProvider), ServiceLifetime.Scoped);
        }
    }
}