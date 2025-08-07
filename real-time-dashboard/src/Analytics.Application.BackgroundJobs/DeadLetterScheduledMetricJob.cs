// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Observability;
using Affiliate.Platform.Extensions.Observability.Builders;
using Hangfire;
using Analytics.Application.BackgroundJobs.Exceptions;
using Analytics.Application.Models.DeadLetter;
using Analytics.Domain.Dapper;
using Analytics.Domain.Models.Configuration;
using Analytics.Domain.Models.Configuration.BackgroundServices.Metrics;
using Analytics.Domain.Observability;
using Analytics.Domain.Observability.Messages;
using Analytics.Domain.Observability.Names;
using BackgroundJob = Affiliate.Platform.Extensions.BackgroundServices.BackgroundJob;

namespace Analytics.Application.BackgroundJobs
{
    internal sealed class DeadLetterScheduledMetricJob : BackgroundJob
    {
        
        private readonly IObservabilityManager<DeadLetterScheduledMetricJob> _observabilityManager;
        private readonly IAnalyticsConfiguration _serviceConfiguration;
        private readonly IStoredProcedureExecutorReturn<DeadLetterCountMetric> _storedProcedureExecutorReturn;

        public DeadLetterScheduledMetricJob(IAnalyticsConfiguration serviceConfiguration,
            IStoredProcedureExecutorReturn<DeadLetterCountMetric> storedProcedureExecutorReturn,
            IObservabilityManager<DeadLetterScheduledMetricJob> observabilityManager)
        {
            _serviceConfiguration = serviceConfiguration;
            _storedProcedureExecutorReturn = storedProcedureExecutorReturn;
            _observabilityManager = observabilityManager.WithTag(Tags.METRICS);
        }
        
        [AutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(timeoutInSeconds: 60)]
        public override async Task ExecuteWork()
        {
            _observabilityManager.LogMessage(TraceMessages.METHOD_STARTED).AsTrace();
            using (ActivityBuilder builder = _observabilityManager.CreateActivity(TraceNames.DEAD_LETTER_METRIC_FETCH).Start())
            {
                try
                {
                    MetricJobConfiguration? configuration = _serviceConfiguration.BackgroundServices.JobConfigurations.MetricsJobConfiguration?[nameof(DeadLetterScheduledMetricJob)];
                    if (configuration == null)
                    {
                        throw new BackgroundJobException(ErrorMessages.BACKGROUND_JOB_CONFIGURATION_NOT_FOUND);
                    }

                    List<DeadLetterCountMetric> records = await _storedProcedureExecutorReturn.Get(configuration.Source.ConnectionString,
                        configuration.JobStoredProcedures["DeadLetterMetricStoredProcedure"], new { });

                    foreach (DeadLetterCountMetric record in records)
                    {
                        _observabilityManager.Gauge.Set(MetricMeasureNames.GaugeDeadLetterQueueItems.Name, record.RecordCount, new[] { record.Name });
                    }

                    builder.AddEvent(EventNames.DEAD_LETTER_METRICS_FETCH_COMPLETED);
                    _observabilityManager.LogMessage(InfoMessages.DEAD_LETTER_METRICS_FETCH_COMPLETED).AsInfo();
                }
                catch (Exception exception)
                {
                    _observabilityManager.LogMessage(ErrorMessages.BACKGROUND_JOB_DEAD_LETTER_METRICS_FETCH_ERROR).AsError(exception);
                    _observabilityManager
                        .Counter
                        .Increment(MetricMeasureNames.CounterNumberOfBackgroundJobExceptions.Name);
                    throw;
                }
            }

        }
    }
}