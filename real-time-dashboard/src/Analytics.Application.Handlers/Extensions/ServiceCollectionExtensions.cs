// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Application.Handlers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            // TODO: Add Handler injection here using standard DI
            return services;
        }
    }
}