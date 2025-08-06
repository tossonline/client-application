-- =============================================
-- Create AnalyticsJobs Table
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
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

PRINT 'AnalyticsJobs table created successfully';