namespace Rydo.Kafka.Client.Configurations.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides extension methods over <see cref="IDependencyResolver"/>
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Resolve an instance of <typeparamref name="T" />.
        /// </summary>
        /// <param name="resolver">Instance of <see cref="IDependencyResolver"/></param>
        /// <typeparam name="T">The type to be resolved</typeparam>
        /// <returns></returns>
        public static T Resolve<T>(this IDependencyResolver resolver) => (T) resolver.Resolve(typeof(T))!;
    }
    
    internal class MicrosoftDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }

        public IEnumerable<object?> ResolveAll(Type type)
        {
            return _serviceProvider.GetServices(type);
        }

        public IDependencyResolverScope CreateScope()
        {
            return new MicrosoftDependencyResolverScope(_serviceProvider.CreateScope());
        }
    }
}