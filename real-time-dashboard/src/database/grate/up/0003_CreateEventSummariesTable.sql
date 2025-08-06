-- =============================================
-- Create EventSummaries Table
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
    CONSTRAINT [UQ_EventSummaries_Date_Type_Banner] UNIQUE ([EventDate], [EventType], [BannerTag])
);

PRINT 'EventSummaries table created successfully';