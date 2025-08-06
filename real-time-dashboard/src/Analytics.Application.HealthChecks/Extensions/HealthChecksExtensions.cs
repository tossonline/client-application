// Copyright (c) DigiOutsource. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Analytics.Domain.Models.Configuration;
using Analytics.Domain.Models.Configuration.Kafka;
using Analytics.Domain.Models.HealthCheck;

namespace Analytics.Application.HealthChecks.Extensions
{
    public static class HealthChecksExtensions
    {
        public static IHealthChecksBuilder AddHealthChecks(this IHealthChecksBuilder healthChecksBuilder, IAnalyticsConfiguration configuration)
        {
            return healthChecksBuilder
                .AddKafkaPublisherCheck(configuration)
                .AddSqlServerPersistenceCheck(configuration);
        }

        private static IHealthChecksBuilder AddKafkaPublisherCheck(this IHealthChecksBuilder healthChecksBuilder, IAnalyticsConfiguration configuration)
        {
            KafkaProducerConfiguration kafkaConfiguration = configuration.KafkaProducers["ProduceHealthCheck"];
            ProducerConfig config = new()
            {
                BootstrapServers = string.Join(",", kafkaConfiguration.Brokers),
                EnableDeliveryReports = true,
                Acks = Acks.Leader,
                SecurityProtocol = string.IsNullOrWhiteSpace(kafkaConfiguration.Username) ? SecurityProtocol.Plaintext : SecurityProtocol.SaslSsl,
                SaslMechanism = string.IsNullOrWhiteSpace(kafkaConfiguration.Username) ? SaslMechanism.Plain : SaslMechanism.ScramSha256,
                SaslUsername = string.IsNullOrWhiteSpace(kafkaConfiguration.Username) ? null : kafkaConfiguration.Username,
                SaslPassword = string.IsNullOrWhiteSpace(kafkaConfiguration.Password) ? null : kafkaConfiguration.Password
            };

            // Replace with a public Kafka health check implementation or stub
            // healthChecksBuilder.AddKafka(new KafkaHealthCheckOptions(
            //     config,
            //     kafkaConfiguration.Topic,
            //     sourceApplication: configuration.LoggerOptions.Source));

            return healthChecksBuilder;
        }

        private static IHealthChecksBuilder AddSqlServerPersistenceCheck(this IHealthChecksBuilder healthChecksBuilder, IAnalyticsConfiguration configuration)
        {
            List<SqlHealthCheck> sqlHealthChecks = BuildUniqueListOfSqlDatabaseDependencies(configuration);
            foreach (SqlHealthCheck sqlHealthCheck in sqlHealthChecks)
            {
                healthChecksBuilder
                    .AddSqlServer(
                        sqlHealthCheck.ConnectionString,
                        name: sqlHealthCheck.Name,
                        failureStatus: HealthStatus.Degraded,
                        tags: sqlHealthCheck.Labels.ToArray());
            }

            return healthChecksBuilder;
        }

        private static List<SqlHealthCheck> BuildUniqueListOfSqlDatabaseDependencies(IAnalyticsConfiguration configuration)
        {
            List<SqlHealthCheck> sqlHealthChecks = BuildListOfSqlDatabaseDependencies(configuration);
            Dictionary<string, SqlHealthCheck> uniqueServerHealthChecks = new();

            sqlHealthChecks.ForEach(healthCheck => { uniqueServerHealthChecks.TryAdd(healthCheck.Server, healthCheck); });

            return uniqueServerHealthChecks.Values.ToList();
        }

        private static List<SqlHealthCheck> BuildListOfSqlDatabaseDependencies(IAnalyticsConfiguration configuration)
        {
            List<SqlHealthCheck> sqlHealthChecks = new()
            {
                new SqlHealthCheck("Persistence", configuration.Persistence.ConnectionString, configuration.Persistence.Server,
                    new List<string> { "Database", "SqlServer", "Persistence" })
            };

            return sqlHealthChecks;
        }
    }
}