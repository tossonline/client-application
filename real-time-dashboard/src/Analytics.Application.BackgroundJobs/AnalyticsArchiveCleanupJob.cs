// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Observability;
using Affiliate.Platform.Extensions.Observability.Builders;
using Hangfire;
using Analytics.Application.BackgroundJobs.Exceptions;
using Analytics.Domain.Dapper;
using Analytics.Domain.Models.Configuration;
using Analytics.Domain.Models.Configuration.BackgroundServices.Cleanup;
using Analytics.Domain.Models.StoredProcedure;
using Analytics.Domain.Observability;
using Analytics.Domain.Observability.Messages;
using Analytics.Domain.Observability.Names;
using BackgroundJob = Affiliate.Platform.Extensions.BackgroundServices.BackgroundJob;

namespace Analytics.Application.BackgroundJobs
{
    internal sealed class AnalyticsArchiveCleanupJob : BackgroundJob
    {
        private readonly IObservabilityManager<AnalyticsArchiveCleanupJob> _observabilityManager;
        private readonly IAnalyticsConfiguration _serviceConfiguration;
        private readonly IStoredProcedureExecutor _storedProcedureExecutor;

        public AnalyticsArchiveCleanupJob(IAnalyticsConfiguration serviceConfiguration,
            IStoredProcedureExecutor storedProcedureExecutor,
            IObservabilityManager<AnalyticsArchiveCleanupJob> observabilityManager)
        {
            _serviceConfiguration = serviceConfiguration;
            _storedProcedureExecutor = storedProcedureExecutor;
            _observabilityManager = observabilityManager.WithTag(Tags.ARCHIVE);
        }

        [AutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(60)]
        public override async Task ExecuteWork()
        {
            _observabilityManager.LogMessage(TraceMessages.METHOD_STARTED).AsTrace();
            using (ActivityBuilder builder = _observabilityManager.CreateActivity(TraceNames.ARCHIVE_CLEANUP).Start())
            {
                try
                {
                    CleanupJobConfiguration? configuration = _serviceConfiguration.BackgroundServices.JobConfigurations.CleanupJobConfiguration?[nameof(AnalyticsArchiveCleanupJob)];
                    if (configuration == null)
                    {
                        throw new BackgroundJobException(ErrorMessages.BACKGROUND_JOB_CONFIGURATION_NOT_FOUND);
                    }
                    
                    await _storedProcedureExecutor.Run(new StoredProcedureOptions
                    {
                        ConnectionString = configuration.Source.ConnectionString,
                        Procedure = configuration.JobStoredProcedures["CleanupStoredProcedure"],
                        Parameters = new
                        {
                            configuration.RetentionPeriodDays,
                            configuration.BatchSize,
                            configuration.BatchDelay
                        }
                    });

                    _observabilityManager
                        .Counter
                        .Increment(MetricMeasureNames.CounterNumberOfArchiveCleanupExecutions.Name);

                    builder.AddEvent(EventNames.ARCHIVE_CLEANUP_COMPLETED);
                    _observabilityManager.LogMessage(InfoMessages.ARCHIVE_CLEANUP_COMPLETED).AsInfo();
                }
                catch (Exception ex)
                {
                    _observabilityManager.LogMessage(ErrorMessages.BACKGROUND_JOB_ARCHIVE_CLEANUP_ERROR).AsError(ex);
                    _observabilityManager
                        .Counter
                        .Increment(MetricMeasureNames.CounterNumberOfBackgroundJobExceptions.Name);

                    throw new BackgroundJobException(ErrorMessages.BACKGROUND_JOB_ARCHIVE_CLEANUP_ERROR, ex);
                }
            }
        }
    }
}