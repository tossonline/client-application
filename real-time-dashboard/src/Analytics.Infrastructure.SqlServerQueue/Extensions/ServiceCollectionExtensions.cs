// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Messaging.Abstractions.Messages;
using Affiliate.Platform.Messaging.SqlServer.Extensions;
using Affiliate.Platform.Messaging.SqlServer.Options;
using Affiliate.Platform.Metrics.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Analytics.Domain.Models.Configuration.SqlQueueConsumer;

namespace Analytics.Infrastructure.SqlServerQueue.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlQueueConsumer<TMessage>(this IServiceCollection serviceCollection,
            SqlQueueWithDeadletterConfiguration sqlQueueWithDeadLetterConfiguration)
            where TMessage : Message
        {
            serviceCollection
                .AddSqlConsumer<TMessage>(options =>
                {
                    options.DatabaseObjectOptions = new DatabaseObjectOptions(sqlQueueWithDeadLetterConfiguration.Queue.Server,
                        sqlQueueWithDeadLetterConfiguration.Queue.Database,
                        sqlQueueWithDeadLetterConfiguration.Queue.Schema,
                        sqlQueueWithDeadLetterConfiguration.Queue.Table);

                    options.AuthenticationOptions = new AuthenticationOptions(sqlQueueWithDeadLetterConfiguration.Queue.Username,
                        sqlQueueWithDeadLetterConfiguration.Queue.Password);

                    options.NoRecordBackoffWait = sqlQueueWithDeadLetterConfiguration.NoRecordBackOffWait;
                    options.RetryOptions = new RetryOptions();

                    options.ConnectionTimeoutSeconds = sqlQueueWithDeadLetterConfiguration.Queue.ConnectionTimeoutSeconds;
                    options.CommandTimeoutSeconds = sqlQueueWithDeadLetterConfiguration.Queue.CommandTimeoutSeconds;

                    options.ObservabilityTag = sqlQueueWithDeadLetterConfiguration.ObservabilityTag;
                })
                .AddSqlDeadLetterProducer<TMessage>(options =>
                {
                    options.DatabaseObjectOptions = new DatabaseObjectOptions(sqlQueueWithDeadLetterConfiguration.DeadLetter.Server,
                        sqlQueueWithDeadLetterConfiguration.DeadLetter.Database,
                        sqlQueueWithDeadLetterConfiguration.DeadLetter.Schema,
                        sqlQueueWithDeadLetterConfiguration.DeadLetter.Table);

                    options.AuthenticationOptions = new AuthenticationOptions(sqlQueueWithDeadLetterConfiguration.DeadLetter.Username,
                        sqlQueueWithDeadLetterConfiguration.DeadLetter.Password);

                    options.ObservabilityTag = sqlQueueWithDeadLetterConfiguration.ObservabilityTag;
                });

            return serviceCollection;
        }

        public static IMetricsBuilder AddSqlConsumerAndProducerMetrics(this IMetricsBuilder builder)
        {
            return builder
                .AddSqlProducerMetrics()
                .AddSqlConsumerMetrics();
        }
    }
}