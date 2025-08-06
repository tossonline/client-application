-- =============================================
-- Create AnalyticsJobs Table
-- Purpose: Store background job execution history and status
-- Database: dbAffiliate_Analytics
-- Script Type: up - one-time migration
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
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

-- Add table description
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Background job execution tracking for analytics processing and maintenance.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'AnalyticsJobs';

PRINT 'AnalyticsJobs table created successfully';