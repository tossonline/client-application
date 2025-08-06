// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.Configuration.BackgroundServices;
using Analytics.Domain.Models.Configuration.Handler;
using Analytics.Domain.Models.Configuration.Kafka;
using Analytics.Domain.Models.Configuration.Logger;
using Analytics.Domain.Models.Configuration.SqlServer;

namespace Analytics.Domain.Models.Configuration
{
    public sealed class AnalyticsConfiguration : IAnalyticsConfiguration
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

        public LoggerOptionsConfiguration LoggerOptions { get; set; }
        public Dictionary<string, HandlerConfiguration> Handlers { get; set; }
        public BackgroundServicesConfiguration BackgroundServices { get; set; }
        public SqlServerConfiguration Persistence { get; set; }
        public Dictionary<string, KafkaConsumerConfiguration> KafkaConsumers { get; set; }
        public Dictionary<string, KafkaProducerConfiguration> KafkaProducers { get; set; }
    }
}