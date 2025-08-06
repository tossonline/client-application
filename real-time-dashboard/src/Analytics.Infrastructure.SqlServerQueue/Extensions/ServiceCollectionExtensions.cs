// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure.SqlServerQueue.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlQueueConsumer(this IServiceCollection serviceCollection)
        {
            // TODO: Register SQL queue consumer using public libraries or custom implementation
            return serviceCollection;
        }
    }
}