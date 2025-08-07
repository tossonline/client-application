// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.BackgroundServices;
using Affiliate.Platform.Extensions.BackgroundServices.Extensions;
using Analytics.Application.BackgroundJobs.Exceptions;
using Analytics.Domain.Models.Configuration;
using Analytics.Domain.Observability.Messages;

namespace Analytics.Application.BackgroundJobs.Builder
{
    public static class BackgroundJobBuilder
    {
        public static void RegisterJobs(IAnalyticsConfiguration serviceConfiguration)
        {
            RegisterBackgroundJob<AnalyticsArchiveJob>("AFFILIATE_PLATFORM_ANALYTICS_ARCHIVE", 
                serviceConfiguration.BackgroundServices.JobConfigurations.ArchivingJobConfiguration?[nameof(AnalyticsArchiveJob)].Schedule ?? string.Empty);
            
            RegisterBackgroundJob<AnalyticsArchiveCleanupJob>("AFFILIATE_PLATFORM_ANALYTICS_ARCHIVE_CLEANUP", 
                serviceConfiguration.BackgroundServices.JobConfigurations.CleanupJobConfiguration?[nameof(AnalyticsArchiveCleanupJob)].Schedule ?? string.Empty);
            
            RegisterBackgroundJob<DeadLetterScheduledMetricJob>("AFFILIATE_PLATFORM_ANALYTICS_DEAD_LETTER_METRICS_FETCH", 
                serviceConfiguration.BackgroundServices.JobConfigurations.MetricsJobConfiguration?[nameof(DeadLetterScheduledMetricJob)].Schedule ?? string.Empty);
        }

        private static void RegisterBackgroundJob<T>(string jobId, string schedule)
            where T : BackgroundJob
        {
            if (string.IsNullOrWhiteSpace(schedule))
            {
                throw new BackgroundJobException(ErrorMessages.BACKGROUND_JOB_SCHEDULE_NOT_FOUND);
            }
            
            BackgroundJobManager
                .Create<T>(job => job.ExecuteWork())
                .OnSchedule(jobId, schedule);
        }
    }
}