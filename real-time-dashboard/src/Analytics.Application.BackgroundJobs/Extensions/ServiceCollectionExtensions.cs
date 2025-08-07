using System;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Application.BackgroundJobs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventAggregation(this IServiceCollection services, TimeSpan interval)
        {
            services.AddHostedService(sp => new EventAggregationService(
                sp.GetRequiredService<ILogger<EventAggregationService>>(),
                sp,
                interval));

            return services;
        }
    }
}