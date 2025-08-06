-- =============================================
-- Analytics Database Schema for dbAffiliate_Analytics
-- =============================================

USE [dbAffiliate_Analytics]
GO

-- =============================================
-- Table: PixelEvents
-- Purpose: Store raw pixel tracking events (visits, registrations, deposits)
-- =============================================
CREATE TABLE [dbo].[PixelEvents] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [EventType] NVARCHAR(50) NOT NULL,
    [PlayerId] NVARCHAR(100) NOT NULL,
    [BannerTag] NVARCHAR(100) NOT NULL,
    [Metadata] NVARCHAR(MAX) NULL, -- JSON string for metadata dictionary
    [SourceIp] NVARCHAR(45) NULL, -- Support IPv6
    [UserAgent] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes for performance
    INDEX [IX_PixelEvents_PlayerId] ([PlayerId]),
    INDEX [IX_PixelEvents_BannerTag] ([BannerTag]),
    INDEX [IX_PixelEvents_EventType] ([EventType]),
    INDEX [IX_PixelEvents_CreatedAt] ([CreatedAt]),
    INDEX [IX_PixelEvents_PlayerId_EventType] ([PlayerId], [EventType]),
    INDEX [IX_PixelEvents_BannerTag_CreatedAt] ([BannerTag], [CreatedAt])
);
GO

-- =============================================
-- Table: Players
-- Purpose: Store player lifecycle and tracking information
-- =============================================
CREATE TABLE [dbo].[Players] (
    [PlayerId] NVARCHAR(100) PRIMARY KEY,
    [FirstSeen] DATETIME2(7) NOT NULL,
    [LastEventAt] DATETIME2(7) NULL,
    [RegistrationAt] DATETIME2(7) NULL,
    [DepositAt] DATETIME2(7) NULL,
    [TotalDeposits] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes for performance
    INDEX [IX_Players_FirstSeen] ([FirstSeen]),
    INDEX [IX_Players_RegistrationAt] ([RegistrationAt]),
    INDEX [IX_Players_DepositAt] ([DepositAt]),
    INDEX [IX_Players_TotalDeposits] ([TotalDeposits])
);
GO

-- =============================================
-- Table: EventSummaries
-- Purpose: Store daily aggregated metrics per campaign/banner
-- =============================================
CREATE TABLE [dbo].[EventSummaries] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [EventDate] DATE NOT NULL,
    [EventType] NVARCHAR(50) NOT NULL,
    [BannerTag] NVARCHAR(100) NOT NULL,
    [Count] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Unique constraint to prevent duplicate daily summaries
    CONSTRAINT [UQ_EventSummaries_Date_Type_Banner] UNIQUE ([EventDate], [EventType], [BannerTag]),
    
    -- Indexes for performance
    INDEX [IX_EventSummaries_EventDate] ([EventDate]),
    INDEX [IX_EventSummaries_BannerTag] ([BannerTag]),
    INDEX [IX_EventSummaries_EventType] ([EventType]),
    INDEX [IX_EventSummaries_BannerTag_EventDate] ([BannerTag], [EventDate]),
    INDEX [IX_EventSummaries_EventType_EventDate] ([EventType], [EventDate])
);
GO

-- =============================================
-- Table: Dashboards
-- Purpose: Store dashboard configurations and metadata
-- =============================================
CREATE TABLE [dbo].[Dashboards] (
    [Id] NVARCHAR(100) PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Configuration] NVARCHAR(MAX) NULL, -- JSON string for dashboard config
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(100) NULL,
    [UpdatedBy] NVARCHAR(100) NULL,
    
    -- Indexes for performance
    INDEX [IX_Dashboards_Name] ([Name]),
    INDEX [IX_Dashboards_IsActive] ([IsActive]),
    INDEX [IX_Dashboards_CreatedAt] ([CreatedAt])
);
GO

-- =============================================
-- Table: CampaignMetrics (for Campaign Aggregate)
-- Purpose: Store calculated campaign performance metrics
-- =============================================
CREATE TABLE [dbo].[CampaignMetrics] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [BannerTag] NVARCHAR(100) NOT NULL,
    [MetricDate] DATE NOT NULL,
    [TotalVisits] INT NOT NULL DEFAULT 0,
    [TotalRegistrations] INT NOT NULL DEFAULT 0,
    [TotalDeposits] INT NOT NULL DEFAULT 0,
    [ConversionRate] DECIMAL(5,4) NOT NULL DEFAULT 0.0000, -- e.g., 0.1234 = 12.34%
    [VisitToRegistrationRate] DECIMAL(5,4) NOT NULL DEFAULT 0.0000,
    [RegistrationToDepositRate] DECIMAL(5,4) NOT NULL DEFAULT 0.0000,
    [CalculatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Unique constraint
    CONSTRAINT [UQ_CampaignMetrics_Banner_Date] UNIQUE ([BannerTag], [MetricDate]),
    
    -- Indexes for performance
    INDEX [IX_CampaignMetrics_BannerTag] ([BannerTag]),
    INDEX [IX_CampaignMetrics_MetricDate] ([MetricDate]),
    INDEX [IX_CampaignMetrics_ConversionRate] ([ConversionRate]),
    INDEX [IX_CampaignMetrics_BannerTag_MetricDate] ([BannerTag], [MetricDate])
);
GO

-- =============================================
-- Table: AnalyticsJobs
-- Purpose: Store background job execution history and status
-- =============================================
CREATE TABLE [dbo].[AnalyticsJobs] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [JobName] NVARCHAR(100) NOT NULL,
    [JobType] NVARCHAR(50) NOT NULL, -- 'Aggregation', 'Cleanup', 'Archive', etc.
    [Status] NVARCHAR(20) NOT NULL, -- 'Running', 'Completed', 'Failed', 'Cancelled'
    [StartedAt] DATETIME2(7) NOT NULL,
    [CompletedAt] DATETIME2(7) NULL,
    [Parameters] NVARCHAR(MAX) NULL, -- JSON string for job parameters
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [RecordsProcessed] BIGINT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes for performance
    INDEX [IX_AnalyticsJobs_JobName] ([JobName]),
    INDEX [IX_AnalyticsJobs_JobType] ([JobType]),
    INDEX [IX_AnalyticsJobs_Status] ([Status]),
    INDEX [IX_AnalyticsJobs_StartedAt] ([StartedAt]),
    INDEX [IX_AnalyticsJobs_JobType_Status] ([JobType], [Status])
);
GO

-- =============================================
-- Table: AnomalyDetections
-- Purpose: Store detected anomalies for analytics
-- =============================================
CREATE TABLE [dbo].[AnomalyDetections] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [DetectionDate] DATE NOT NULL,
    [BannerTag] NVARCHAR(100) NULL,
    [MetricType] NVARCHAR(50) NOT NULL, -- 'Visits', 'Registrations', 'Deposits'
    [CurrentValue] INT NOT NULL,
    [ExpectedValue] DECIMAL(10,2) NOT NULL,
    [StandardDeviation] DECIMAL(10,2) NOT NULL,
    [ZScore] DECIMAL(10,4) NOT NULL,
    [AnomalyType] NVARCHAR(20) NOT NULL, -- 'spike', 'drop'
    [IsAnomaly] BIT NOT NULL,
    [Severity] NVARCHAR(10) NOT NULL DEFAULT 'Low', -- 'Low', 'Medium', 'High'
    [DetectedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes for performance
    INDEX [IX_AnomalyDetections_DetectionDate] ([DetectionDate]),
    INDEX [IX_AnomalyDetections_BannerTag] ([BannerTag]),
    INDEX [IX_AnomalyDetections_IsAnomaly] ([IsAnomaly]),
    INDEX [IX_AnomalyDetections_Severity] ([Severity]),
    INDEX [IX_AnomalyDetections_BannerTag_DetectionDate] ([BannerTag], [DetectionDate])
);
GO

-- =============================================
-- Table: GeoAnalytics
-- Purpose: Store geographic analytics data
-- =============================================
CREATE TABLE [dbo].[GeoAnalytics] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [AnalysisDate] DATE NOT NULL,
    [Country] NVARCHAR(100) NOT NULL,
    [BannerTag] NVARCHAR(100) NULL,
    [TotalEvents] INT NOT NULL DEFAULT 0,
    [Visits] INT NOT NULL DEFAULT 0,
    [Registrations] INT NOT NULL DEFAULT 0,
    [Deposits] INT NOT NULL DEFAULT 0,
    [ConversionRate] DECIMAL(5,4) NOT NULL DEFAULT 0.0000,
    [CalculatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes for performance
    INDEX [IX_GeoAnalytics_AnalysisDate] ([AnalysisDate]),
    INDEX [IX_GeoAnalytics_Country] ([Country]),
    INDEX [IX_GeoAnalytics_BannerTag] ([BannerTag]),
    INDEX [IX_GeoAnalytics_Country_AnalysisDate] ([Country], [AnalysisDate])
);
GO

-- =============================================
-- Views for Analytics
-- =============================================

-- Campaign Performance Summary View
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
    END AS ConversionRate
FROM [dbo].[EventSummaries] es
GROUP BY es.BannerTag, es.EventDate;
GO

-- Player Funnel Analysis View
CREATE VIEW [dbo].[vw_PlayerFunnelAnalysis]
AS
SELECT 
    p.PlayerId,
    p.FirstSeen,
    p.RegistrationAt,
    p.DepositAt,
    p.TotalDeposits,
    CASE WHEN p.RegistrationAt IS NOT NULL THEN 1 ELSE 0 END AS IsRegistered,
    CASE WHEN p.DepositAt IS NOT NULL THEN 1 ELSE 0 END AS HasDeposited,
    DATEDIFF(day, p.FirstSeen, p.RegistrationAt) AS DaysToRegistration,
    DATEDIFF(day, p.RegistrationAt, p.DepositAt) AS DaysToFirstDeposit
FROM [dbo].[Players] p;
GO

-- Daily Metrics Summary View
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
    AVG(CASE WHEN es.EventType = 'deposit' THEN CAST(es.Count AS DECIMAL(10,2)) ELSE NULL END) AS AvgDepositsPerCampaign
FROM [dbo].[EventSummaries] es
GROUP BY es.EventDate;
GO

-- =============================================
-- Stored Procedures for Common Operations
-- =============================================

-- Upsert Event Summary
CREATE PROCEDURE [dbo].[sp_UpsertEventSummary]
    @EventDate DATE,
    @EventType NVARCHAR(50),
    @BannerTag NVARCHAR(100),
    @Count INT
AS
BEGIN
    SET NOCOUNT ON;
    
    MERGE [dbo].[EventSummaries] AS target
    USING (SELECT @EventDate AS EventDate, @EventType AS EventType, @BannerTag AS BannerTag, @Count AS Count) AS source
    ON target.EventDate = source.EventDate 
       AND target.EventType = source.EventType 
       AND target.BannerTag = source.BannerTag
    WHEN MATCHED THEN
        UPDATE SET 
            Count = source.Count,
            UpdatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (EventDate, EventType, BannerTag, Count, CreatedAt, UpdatedAt)
        VALUES (source.EventDate, source.EventType, source.BannerTag, source.Count, GETUTCDATE(), GETUTCDATE());
END;
GO

-- Get Campaign Performance
CREATE PROCEDURE [dbo].[sp_GetCampaignPerformance]
    @BannerTag NVARCHAR(100),
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        BannerTag,
        EventDate,
        Visits,
        Registrations,
        Deposits,
        ConversionRate
    FROM [dbo].[vw_CampaignPerformanceSummary]
    WHERE BannerTag = @BannerTag
      AND EventDate BETWEEN @FromDate AND @ToDate
    ORDER BY EventDate;
END;
GO

-- Calculate Campaign Metrics
CREATE PROCEDURE [dbo].[sp_CalculateCampaignMetrics]
    @BannerTag NVARCHAR(100) = NULL,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    
    SET @SQL = N'
    INSERT INTO [dbo].[CampaignMetrics] (BannerTag, MetricDate, TotalVisits, TotalRegistrations, TotalDeposits, ConversionRate, VisitToRegistrationRate, RegistrationToDepositRate)
    SELECT 
        BannerTag,
        EventDate,
        Visits,
        Registrations,
        Deposits,
        ConversionRate,
        CASE WHEN Visits > 0 THEN CAST(Registrations AS DECIMAL(10,4)) / Visits ELSE 0.0000 END,
        CASE WHEN Registrations > 0 THEN CAST(Deposits AS DECIMAL(10,4)) / Registrations ELSE 0.0000 END
    FROM [dbo].[vw_CampaignPerformanceSummary]
    WHERE EventDate BETWEEN @FromDate AND @ToDate';
    
    IF @BannerTag IS NOT NULL
    BEGIN
        SET @SQL = @SQL + N' AND BannerTag = @BannerTag';
    END;
    
    SET @SQL = @SQL + N'
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[CampaignMetrics] cm 
        WHERE cm.BannerTag = [dbo].[vw_CampaignPerformanceSummary].BannerTag 
          AND cm.MetricDate = [dbo].[vw_CampaignPerformanceSummary].EventDate
    )';
    
    EXEC sp_executesql @SQL, N'@BannerTag NVARCHAR(100), @FromDate DATE, @ToDate DATE', @BannerTag, @FromDate, @ToDate;
END;
GO

-- =============================================
-- Functions for Analytics
-- =============================================

-- Calculate Conversion Rate
CREATE FUNCTION [dbo].[fn_CalculateConversionRate]
(
    @Conversions INT,
    @TotalEvents INT
)
RETURNS DECIMAL(5,4)
AS
BEGIN
    RETURN CASE 
        WHEN @TotalEvents > 0 THEN CAST(@Conversions AS DECIMAL(10,4)) / @TotalEvents
        ELSE 0.0000
    END;
END;
GO

-- Get Date Range for Trend Analysis
CREATE FUNCTION [dbo].[fn_GetDateRange]
(
    @Days INT
)
RETURNS TABLE
AS
RETURN
(
    WITH DateRange AS (
        SELECT CAST(GETDATE() AS DATE) AS DateValue, 0 AS DayOffset
        UNION ALL
        SELECT DATEADD(DAY, -1, DateValue), DayOffset + 1
        FROM DateRange
        WHERE DayOffset < @Days - 1
    )
    SELECT DateValue FROM DateRange
);
GO

-- =============================================
-- Sample Data for Testing (Optional)
-- =============================================

-- Insert sample dashboards
INSERT INTO [dbo].[Dashboards] (Id, Name, Description, Configuration, CreatedBy)
VALUES 
    ('campaign-overview', 'Campaign Overview Dashboard', 'Main dashboard for campaign performance monitoring', '{"layout":"grid","widgets":["visits","conversions","trends"]}', 'system'),
    ('player-analytics', 'Player Analytics Dashboard', 'Dashboard for player behavior and lifecycle analysis', '{"layout":"tabs","widgets":["funnel","retention","geography"]}', 'system'),
    ('real-time-metrics', 'Real-time Metrics Dashboard', 'Live dashboard for real-time analytics', '{"layout":"stream","widgets":["live-events","live-metrics"]}', 'system');
GO

PRINT 'Analytics database schema created successfully for dbAffiliate_Analytics!';
PRINT 'Tables created: PixelEvents, Players, EventSummaries, Dashboards, CampaignMetrics, AnalyticsJobs, AnomalyDetections, GeoAnalytics';
PRINT 'Views created: vw_CampaignPerformanceSummary, vw_PlayerFunnelAnalysis, vw_DailyMetricsSummary';
PRINT 'Stored procedures created: sp_UpsertEventSummary, sp_GetCampaignPerformance, sp_CalculateCampaignMetrics';
PRINT 'Functions created: fn_CalculateConversionRate, fn_GetDateRange';