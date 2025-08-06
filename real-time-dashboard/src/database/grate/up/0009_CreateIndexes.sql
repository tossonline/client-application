-- =============================================
-- Create Indexes for Performance Optimization
-- Purpose: Add indexes for all tables to improve query performance
-- =============================================

-- PixelEvents Indexes
CREATE INDEX [IX_PixelEvents_PlayerId] ON [dbo].[PixelEvents] ([PlayerId]);
CREATE INDEX [IX_PixelEvents_BannerTag] ON [dbo].[PixelEvents] ([BannerTag]);
CREATE INDEX [IX_PixelEvents_EventType] ON [dbo].[PixelEvents] ([EventType]);
CREATE INDEX [IX_PixelEvents_CreatedAt] ON [dbo].[PixelEvents] ([CreatedAt]);
CREATE INDEX [IX_PixelEvents_PlayerId_EventType] ON [dbo].[PixelEvents] ([PlayerId], [EventType]);
CREATE INDEX [IX_PixelEvents_BannerTag_CreatedAt] ON [dbo].[PixelEvents] ([BannerTag], [CreatedAt]);

-- Players Indexes
CREATE INDEX [IX_Players_FirstSeen] ON [dbo].[Players] ([FirstSeen]);
CREATE INDEX [IX_Players_RegistrationAt] ON [dbo].[Players] ([RegistrationAt]);
CREATE INDEX [IX_Players_DepositAt] ON [dbo].[Players] ([DepositAt]);
CREATE INDEX [IX_Players_TotalDeposits] ON [dbo].[Players] ([TotalDeposits]);

-- EventSummaries Indexes
CREATE INDEX [IX_EventSummaries_EventDate] ON [dbo].[EventSummaries] ([EventDate]);
CREATE INDEX [IX_EventSummaries_BannerTag] ON [dbo].[EventSummaries] ([BannerTag]);
CREATE INDEX [IX_EventSummaries_EventType] ON [dbo].[EventSummaries] ([EventType]);
CREATE INDEX [IX_EventSummaries_BannerTag_EventDate] ON [dbo].[EventSummaries] ([BannerTag], [EventDate]);
CREATE INDEX [IX_EventSummaries_EventType_EventDate] ON [dbo].[EventSummaries] ([EventType], [EventDate]);

-- Dashboards Indexes
CREATE INDEX [IX_Dashboards_Name] ON [dbo].[Dashboards] ([Name]);
CREATE INDEX [IX_Dashboards_IsActive] ON [dbo].[Dashboards] ([IsActive]);
CREATE INDEX [IX_Dashboards_CreatedAt] ON [dbo].[Dashboards] ([CreatedAt]);

-- CampaignMetrics Indexes
CREATE INDEX [IX_CampaignMetrics_BannerTag] ON [dbo].[CampaignMetrics] ([BannerTag]);
CREATE INDEX [IX_CampaignMetrics_MetricDate] ON [dbo].[CampaignMetrics] ([MetricDate]);
CREATE INDEX [IX_CampaignMetrics_ConversionRate] ON [dbo].[CampaignMetrics] ([ConversionRate]);
CREATE INDEX [IX_CampaignMetrics_BannerTag_MetricDate] ON [dbo].[CampaignMetrics] ([BannerTag], [MetricDate]);

-- AnomalyDetections Indexes
CREATE INDEX [IX_AnomalyDetections_DetectionDate] ON [dbo].[AnomalyDetections] ([DetectionDate]);
CREATE INDEX [IX_AnomalyDetections_BannerTag] ON [dbo].[AnomalyDetections] ([BannerTag]);
CREATE INDEX [IX_AnomalyDetections_IsAnomaly] ON [dbo].[AnomalyDetections] ([IsAnomaly]);
CREATE INDEX [IX_AnomalyDetections_Severity] ON [dbo].[AnomalyDetections] ([Severity]);
CREATE INDEX [IX_AnomalyDetections_BannerTag_DetectionDate] ON [dbo].[AnomalyDetections] ([BannerTag], [DetectionDate]);

-- GeoAnalytics Indexes
CREATE INDEX [IX_GeoAnalytics_AnalysisDate] ON [dbo].[GeoAnalytics] ([AnalysisDate]);
CREATE INDEX [IX_GeoAnalytics_Country] ON [dbo].[GeoAnalytics] ([Country]);
CREATE INDEX [IX_GeoAnalytics_BannerTag] ON [dbo].[GeoAnalytics] ([BannerTag]);
CREATE INDEX [IX_GeoAnalytics_Country_AnalysisDate] ON [dbo].[GeoAnalytics] ([Country], [AnalysisDate]);

-- AnalyticsJobs Indexes
CREATE INDEX [IX_AnalyticsJobs_JobName] ON [dbo].[AnalyticsJobs] ([JobName]);
CREATE INDEX [IX_AnalyticsJobs_JobType] ON [dbo].[AnalyticsJobs] ([JobType]);
CREATE INDEX [IX_AnalyticsJobs_Status] ON [dbo].[AnalyticsJobs] ([Status]);
CREATE INDEX [IX_AnalyticsJobs_StartedAt] ON [dbo].[AnalyticsJobs] ([StartedAt]);
CREATE INDEX [IX_AnalyticsJobs_JobType_Status] ON [dbo].[AnalyticsJobs] ([JobType], [Status]);

PRINT 'All indexes created successfully';