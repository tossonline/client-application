// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumer(this IServiceCollection serviceCollection)
        {
            // TODO: Register Kafka consumer using MassTransit or other public library
            return serviceCollection;
        }

        public static IServiceCollection AddKafkaProducer(this IServiceCollection serviceCollection)
        {
            // TODO: Register Kafka producer using MassTransit or other public library
            return serviceCollection;
        }
    }
}