-- =============================================
-- Create EventSummaries Table
-- Purpose: Store daily aggregated metrics per campaign/banner
-- Database: dbAffiliate_Analytics
-- Script Type: up - one-time migration
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
    CONSTRAINT [UQ_EventSummaries_Date_Type_Banner] UNIQUE ([EventDate], [EventType], [BannerTag])
);

-- Add table description
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Daily aggregated event metrics used by Campaign Analytics and Metrics aggregates.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'EventSummaries';

PRINT 'EventSummaries table created successfully';