-- =============================================
-- Create GeoAnalytics Table
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
    [CalculatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

PRINT 'GeoAnalytics table created successfully';