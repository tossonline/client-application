-- =============================================
-- Create CampaignMetrics Table
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
    CONSTRAINT [UQ_CampaignMetrics_Banner_Date] UNIQUE ([BannerTag], [MetricDate])
);

PRINT 'CampaignMetrics table created successfully';