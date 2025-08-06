// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Analytics.Domain.Models.Configuration;

namespace Analytics.Application.BackgroundJobs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IAnalyticsConfiguration serviceConfiguration)
        {
            // TODO: Register background job services using standard DI
            return services;
        }
    }
}