-- =============================================
-- Create AnomalyDetections Table
-- Purpose: Store detected anomalies for analytics
-- Database: dbAffiliate_Analytics
-- Script Type: up - one-time migration
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
    [DetectedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

-- Add table description
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Statistical anomaly detection results from AnalyticsAggregate processing.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'AnomalyDetections';

PRINT 'AnomalyDetections table created successfully';