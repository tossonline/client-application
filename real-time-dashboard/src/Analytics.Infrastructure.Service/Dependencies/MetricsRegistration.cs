// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Observability;
using Affiliate.Platform.Extensions.Service.Configuration;
using Affiliate.Platform.Metrics.Abstractions;
using Analytics.Application.Handlers.Extensions;
using Analytics.Application.Metrics.Extensions;
using Analytics.Infrastructure.Kafka.Extensions;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class MetricsRegistration
    {
        public static void RegisterDependencies(IMetricsBuilder builder, IServiceConfiguration serviceConfiguration)
        {
            ObservabilityManagerConfiguration.Service = "analytics";
            ObservabilityManagerConfiguration.Context = "affiliate";

            builder
                .AddHandlerMetrics()
                .AddDomainMetrics()
                .AddKafkaProducerAndConsumerMetrics();
        }
    }
}