// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Application.Handlers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            // Register command handlers
            services.AddScoped<IIngestPixelEventHandler, IngestPixelEventHandler>();
            services.AddScoped<IAggregateEventsHandler, AggregateEventsHandler>();
            
            return services;
        }
    }
}