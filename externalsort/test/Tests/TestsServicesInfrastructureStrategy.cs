using System;
using DataAccess;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    /// <summary>
    /// Class represents tests-specific registrations of services into <see cref="IServiceCollection"/>
    /// </summary>
    public sealed class TestsServicesInfrastructureStrategy: IDependenciesRegistrationStrategy
    {
        public void Register(Action<Type, Type, ServiceLifetime> action)
        {
            action(typeof(TextFileDataProvider), typeof(TextFileDataProvider), ServiceLifetime.Scoped);
        }
    }
}