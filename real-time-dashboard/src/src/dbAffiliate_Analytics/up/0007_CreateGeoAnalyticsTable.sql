-- =============================================
-- Create GeoAnalytics Table
-- Purpose: Store geographic analytics data
-- Database: dbAffiliate_Analytics
-- Script Type: up - one-time migration
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
    [CalculatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

-- Add table description
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Geographic distribution analytics data from AnalyticsAggregate geographic analysis.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'GeoAnalytics';

PRINT 'GeoAnalytics table created successfully';