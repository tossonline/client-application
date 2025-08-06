// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Service.Configuration;
using Analytics.Domain.Models.Configuration.BackgroundServices;
using Analytics.Domain.Models.Configuration.Handler;
using Analytics.Domain.Models.Configuration.Kafka;
using Analytics.Domain.Models.Configuration.Logger;
using Analytics.Domain.Models.Configuration.SqlServer;

namespace Analytics.Domain.Models.Configuration
{
    public sealed class AnalyticsConfiguration : ServiceConfiguration, IAnalyticsConfiguration
    {
        public AnalyticsConfiguration()
        {
            LoggerOptions = new LoggerOptionsConfiguration();
            BackgroundServices = new BackgroundServicesConfiguration();
            Persistence = new SqlServerConfiguration();
            Handlers = new Dictionary<string, HandlerConfiguration>();
            KafkaProducers = new Dictionary<string, KafkaProducerConfiguration>();
            KafkaConsumers = new Dictionary<string, KafkaConsumerConfiguration>();
        }

        public LoggerOptionsConfiguration LoggerOptions { get; init; }
        public Dictionary<string, HandlerConfiguration> Handlers { get; init; }
        public BackgroundServicesConfiguration BackgroundServices { get; init; }
        public SqlServerConfiguration Persistence { get; init; }
        public Dictionary<string, KafkaConsumerConfiguration> KafkaConsumers { get; init; }
        public Dictionary<string, KafkaProducerConfiguration> KafkaProducers { get; init; }
    }
}