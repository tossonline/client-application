// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.Configuration.BackgroundServices;
using Analytics.Domain.Models.Configuration.Handler;
using Analytics.Domain.Models.Configuration.Kafka;
using Analytics.Domain.Models.Configuration.Logger;
using Analytics.Domain.Models.Configuration.SqlServer;

namespace Analytics.Domain.Models.Configuration
{
    public interface IAnalyticsConfiguration
    {
        LoggerOptionsConfiguration LoggerOptions { get; }
        Dictionary<string, HandlerConfiguration> Handlers { get; }
        BackgroundServicesConfiguration BackgroundServices { get; }
        SqlServerConfiguration Persistence { get; }
        Dictionary<string, KafkaConsumerConfiguration> KafkaConsumers { get; }
        Dictionary<string, KafkaProducerConfiguration> KafkaProducers { get; }
    }
}