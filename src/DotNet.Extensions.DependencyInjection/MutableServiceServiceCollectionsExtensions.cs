using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Extensions.DependencyInjection
{
    public static class MutableServiceServiceCollectionsExtensions
    {
        public static IServiceCollection AddMutable<TService>(this IServiceCollection services, Func<TService> factory, Func<TService, IChangeToken> tokenProvider)
        {
            services.AddSingleton<IMutableService<TService>>(new MutableService<TService, TService>(factory, tokenProvider));
            return services;
        }

        public static IServiceCollection AddMutable<TInterface, TService>(this IServiceCollection services, Func<TInterface> factory, Func<TInterface, IChangeToken> tokenProvider)
            where TService : TInterface
        {
            services.AddSingleton<IMutableService<TInterface>>(new MutableService<TInterface, TService>(factory, tokenProvider));
            return services;
        }

        public static IServiceCollection AddMutable<TService>(this IServiceCollection services, TService service)
        {
            var serviceMutator = new ServiceMutator<TService, TService>(service);
            services.AddSingleton<IMutableService<TService>>(serviceMutator);
            services.AddSingleton<IServiceMutator<TService>>(serviceMutator);
            return services;
        }

        public static IServiceCollection AddMutable<TInterface, TService>(this IServiceCollection services, TInterface service)
            where TService : TInterface
        {
            var serviceMutator = new ServiceMutator<TInterface, TService>(service);
            services.AddSingleton<IMutableService<TInterface>>(serviceMutator);
            services.AddSingleton<IServiceMutator<TInterface>>(serviceMutator);
            return services;
        }
    }
}
