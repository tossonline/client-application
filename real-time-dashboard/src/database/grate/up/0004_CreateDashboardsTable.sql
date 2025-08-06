-- =============================================
-- Create Dashboards Table
-- Purpose: Store dashboard configurations and metadata
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

PRINT 'Dashboards table created successfully';