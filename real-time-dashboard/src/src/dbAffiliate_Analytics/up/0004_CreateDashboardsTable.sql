-- =============================================
-- Create Dashboards Table
-- Purpose: Store dashboard configurations and metadata
-- Database: dbAffiliate_Analytics
-- Script Type: up - one-time migration
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
    [UpdatedBy] NVARCHAR(100) NULL
);

-- Add table description
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Configuration storage for analytics dashboards and UI components.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Dashboards';

PRINT 'Dashboards table created successfully';