-- =============================================
-- Daily Metrics Summary View
-- Purpose: Daily aggregated metrics across all campaigns
-- Database: dbAffiliate_Analytics
-- Script Type: views - recreated on each run
-- Used by: MetricsAggregate and AnalyticsAggregate for daily analytics
-- =============================================

CREATE VIEW [dbo].[vw_DailyMetricsSummary]
AS
SELECT 
    es.EventDate,
    COUNT(DISTINCT es.BannerTag) AS UniqueCampaigns,
    SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END) AS TotalVisits,
    SUM(CASE WHEN es.EventType = 'registration' THEN es.Count ELSE 0 END) AS TotalRegistrations,
    SUM(CASE WHEN es.EventType = 'deposit' THEN es.Count ELSE 0 END) AS TotalDeposits,
    AVG(CASE WHEN es.EventType = 'visit' THEN CAST(es.Count AS DECIMAL(10,2)) ELSE NULL END) AS AvgVisitsPerCampaign,
    AVG(CASE WHEN es.EventType = 'registration' THEN CAST(es.Count AS DECIMAL(10,2)) ELSE NULL END) AS AvgRegistrationsPerCampaign,
    AVG(CASE WHEN es.EventType = 'deposit' THEN CAST(es.Count AS DECIMAL(10,2)) ELSE NULL END) AS AvgDepositsPerCampaign,
    CASE 
        WHEN SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END) > 0 
        THEN CAST(SUM(CASE WHEN es.EventType = 'deposit' THEN es.Count ELSE 0 END) AS DECIMAL(10,4)) / 
             SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END)
        ELSE 0.0000
    END AS OverallConversionRate,
    CASE 
        WHEN SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END) > 0 
        THEN CAST(SUM(CASE WHEN es.EventType = 'registration' THEN es.Count ELSE 0 END) AS DECIMAL(10,4)) / 
             SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END)
        ELSE 0.0000
    END AS OverallRegistrationRate
FROM [dbo].[EventSummaries] es
GROUP BY es.EventDate;