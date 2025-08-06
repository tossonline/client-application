// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Application.Translators.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTranslators(this IServiceCollection services)
        {
            // TODO: Add translator injections here using standard DI
            return services;
        }

        private static IServiceCollection AddTranslator<T, TS>(this IServiceCollection serviceCollection) where T : class where TS : class, T
        {
            return serviceCollection.AddTransient<T, TS>();
        }
    }
}