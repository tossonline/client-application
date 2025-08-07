// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Logging.Messaging.Messages;
using Affiliate.Platform.Extensions.Service.Configuration;
using Analytics.Application.BackgroundJobs.Extensions;
using Analytics.Application.Handlers.Extensions;
using Analytics.Application.Models.DeadLetter;
using Analytics.Application.Translators.Extensions;
using Analytics.Domain.Models.Configuration;
using Analytics.Infrastructure.Dapper.Extensions;
using Analytics.Infrastructure.Kafka.Extensions;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class DependencyRegistration
    {
        public static void RegisterDependencies(IServiceCollection service, IServiceConfiguration serviceConfiguration)
        {
            service
                .AddSingleton((IAnalyticsConfiguration)serviceConfiguration)
                .AddStoredProcedureExecutor()
                .AddStoredProcedureExecutor<DeadLetterCountMetric>()
                .AddBackgroundJobs((IAnalyticsConfiguration)serviceConfiguration)
                .AddKafkaProducers((IAnalyticsConfiguration)serviceConfiguration)
                .AddHandlers()
                .AddTranslators();
        }

        private static IServiceCollection AddKafkaProducers(this IServiceCollection service, IAnalyticsConfiguration serviceConfiguration)
        {
            return service
                .AddKafkaProducer<LogMessage>(serviceConfiguration.KafkaProducers[nameof(LogMessage)]);
        }
    }
}