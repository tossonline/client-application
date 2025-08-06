-- =============================================
-- Campaign Performance Summary View
-- Purpose: Aggregated campaign performance with conversion rates
-- Database: dbAffiliate_Analytics
-- Script Type: views - recreated on each run
-- Used by: CampaignAggregate for performance analysis
-- =============================================

CREATE VIEW [dbo].[vw_CampaignPerformanceSummary]
AS
SELECT 
    es.BannerTag,
    es.EventDate,
    SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END) AS Visits,
    SUM(CASE WHEN es.EventType = 'registration' THEN es.Count ELSE 0 END) AS Registrations,
    SUM(CASE WHEN es.EventType = 'deposit' THEN es.Count ELSE 0 END) AS Deposits,
    CASE 
        WHEN SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END) > 0 
        THEN CAST(SUM(CASE WHEN es.EventType = 'deposit' THEN es.Count ELSE 0 END) AS DECIMAL(10,4)) / 
             SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END)
        ELSE 0.0000
    END AS ConversionRate,
    CASE 
        WHEN SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END) > 0 
        THEN CAST(SUM(CASE WHEN es.EventType = 'registration' THEN es.Count ELSE 0 END) AS DECIMAL(10,4)) / 
             SUM(CASE WHEN es.EventType = 'visit' THEN es.Count ELSE 0 END)
        ELSE 0.0000
    END AS VisitToRegistrationRate,
    CASE 
        WHEN SUM(CASE WHEN es.EventType = 'registration' THEN es.Count ELSE 0 END) > 0 
        THEN CAST(SUM(CASE WHEN es.EventType = 'deposit' THEN es.Count ELSE 0 END) AS DECIMAL(10,4)) / 
             SUM(CASE WHEN es.EventType = 'registration' THEN es.Count ELSE 0 END)
        ELSE 0.0000
    END AS RegistrationToDepositRate
FROM [dbo].[EventSummaries] es
GROUP BY es.BannerTag, es.EventDate;