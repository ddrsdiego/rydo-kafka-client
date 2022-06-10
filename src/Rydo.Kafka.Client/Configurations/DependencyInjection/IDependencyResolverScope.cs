namespace Rydo.Kafka.Client.Configurations.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public interface IDependencyResolverScope : IDisposable
    {
        /// <summary>
        /// Gets the dependency injection resolver
        /// </summary>
        IDependencyResolver Resolver { get; }
    }
    
    internal class MicrosoftDependencyResolverScope : IDependencyResolverScope
    {
        private readonly IServiceScope _scope;

        public MicrosoftDependencyResolverScope(IServiceScope scope)
        {
            _scope = scope;
            Resolver = new MicrosoftDependencyResolver(scope.ServiceProvider);
        }

        public IDependencyResolver Resolver { get; }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}