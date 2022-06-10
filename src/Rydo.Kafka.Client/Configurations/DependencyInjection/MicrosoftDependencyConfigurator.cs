namespace Rydo.Kafka.Client.Configurations.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    internal class MicrosoftDependencyConfigurator : IDependencyConfigurator
    {
        private readonly IServiceCollection _services;

        public MicrosoftDependencyConfigurator(IServiceCollection services)
        {
            _services = services;
            _services.AddSingleton<IDependencyResolver>(provider => new MicrosoftDependencyResolver(provider));
        }

        public IDependencyConfigurator Add(
            Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime)
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    serviceType,
                    implementationType,
                    lifetime));

            return this;
        }

        public IDependencyConfigurator Add<TService, TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    typeof(TService),
                    typeof(TImplementation),
                    lifetime));

            return this;
        }

        public IDependencyConfigurator Add<TService>(ServiceLifetime lifetime)
            where TService : class
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    typeof(TService),
                    typeof(TService),
                    lifetime));

            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class
        {
            _services.AddSingleton(service);
            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            ServiceLifetime lifetime)
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    serviceType,
                    provider => factory(new MicrosoftDependencyResolver(provider)),
                    lifetime));

            return this;
        }
    }
}