// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Application.BackgroundJobs.Exceptions;
using Analytics.Domain.Models.Configuration;
using Analytics.Domain.Observability.Messages;

namespace Analytics.Application.BackgroundJobs.Builder
{
    public static class BackgroundJobBuilder
    {
        public static void RegisterJobs(IAnalyticsConfiguration serviceConfiguration)
        {
            // TODO: Replace with public background job scheduler (e.g., Hangfire, Quartz.NET) or custom implementation
            // Example stub:
            // ScheduleJob<AnalyticsArchiveJob>(serviceConfiguration.BackgroundServices.JobConfigurations.ArchivingJobConfiguration?[nameof(AnalyticsArchiveJob)].Schedule);
            // ScheduleJob<AnalyticsArchiveCleanupJob>(serviceConfiguration.BackgroundServices.JobConfigurations.CleanupJobConfiguration?[nameof(AnalyticsArchiveCleanupJob)].Schedule);
            // ScheduleJob<DeadLetterScheduledMetricJob>(serviceConfiguration.BackgroundServices.JobConfigurations.MetricsJobConfiguration?[nameof(DeadLetterScheduledMetricJob)].Schedule);
        }

        // Stub for scheduling jobs
        private static void ScheduleJob<T>(string schedule)
        {
            if (string.IsNullOrWhiteSpace(schedule))
            {
                throw new BackgroundJobException(ErrorMessages.BACKGROUND_JOB_SCHEDULE_NOT_FOUND);
            }
            // Implement scheduling logic here
        }
    }
}