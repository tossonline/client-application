// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Messaging.Abstractions.Extensions;
using Affiliate.Platform.Messaging.Abstractions.Messages;
using Affiliate.Platform.Messaging.Kafka.Enumerations;
using Affiliate.Platform.Messaging.Kafka.Extensions;
using Affiliate.Platform.Messaging.Kafka.Options;
using Affiliate.Platform.Metrics.Abstractions;
using Affiliate.Platform.Metrics.Kafka.Prometheus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Analytics.Domain.Models.Configuration.Kafka;

namespace Analytics.Infrastructure.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaConsumer<TMessage>(this IServiceCollection serviceCollection, KafkaConsumerConfiguration configuration)
            where TMessage : Message
        {
            serviceCollection
                .AddMessaging()
                .AddKafkaConsumer<TMessage>(options =>
                {
                    options.AuthenticationOptions = new AuthenticationOptions(configuration.Username, configuration.Password);
                    options.Brokers = configuration.Brokers;
                    options.Topics = configuration.Topics;
                    options.GroupId = configuration.GroupId;
                    options.Deserializer = (ValueDeserializer)Enum
                        .Parse(typeof(ValueDeserializer),
                            configuration.Deserializer, true);
                    options.SchemaRegistryUrl = configuration.SchemaRegistryUrl;
                    options.BatchWindowTimeoutMilliSeconds = 1000;
                });

            return serviceCollection;
        }

        public static IServiceCollection AddKafkaProducer<TMessage>(this IServiceCollection serviceCollection, KafkaProducerConfiguration configuration)
            where TMessage : Message
        {
            serviceCollection
                .AddMessaging()
                .AddKafkaPublisher<TMessage>(options =>
                {
                    options.AuthenticationOptions = new AuthenticationOptions(configuration.Username, configuration.Password);
                    options.Brokers = configuration.Brokers;
                    options.Topic = configuration.Topic;
                    options.Serializer = (ValueSerializer)Enum
                        .Parse(typeof(ValueSerializer),
                            configuration.Serializer, true);
                    options.SchemaRegistryUrl = configuration.SchemaRegistryUrl;
                });

            return serviceCollection;
        }

        public static IServiceCollection AddKafkaStatisticsHandlers(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddKafkaPublisherStatisticsHandlers()
                .AddKafkaConsumerStatisticsHandlers();
        }

        public static IMetricsBuilder AddKafkaProducerAndConsumerMetrics(this IMetricsBuilder builder)
        {
            builder
                .AddKafkaMetrics();

            return builder;
        }
    }
}