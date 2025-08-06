-- =============================================
-- Create CampaignMetrics Table
-- Purpose: Store calculated campaign performance metrics
-- Database: dbAffiliate_Analytics
-- Script Type: up - one-time migration
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
    CONSTRAINT [UQ_CampaignMetrics_Banner_Date] UNIQUE ([BannerTag], [MetricDate])
);

-- Add table description
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Pre-calculated campaign performance metrics used by CampaignAggregate.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'CampaignMetrics';

PRINT 'CampaignMetrics table created successfully';