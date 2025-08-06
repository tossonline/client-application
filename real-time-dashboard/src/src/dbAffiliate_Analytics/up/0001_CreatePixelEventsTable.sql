-- =============================================
-- Create PixelEvents Table
-- Purpose: Store raw pixel tracking events (visits, registrations, deposits)
-- Database: dbAffiliate_Analytics
-- Migration Tool: Grate (https://erikbra.github.io/grate/)
-- Script Type: up - one-time migration
-- =============================================

-- Verify we're in the correct database
IF DB_NAME() != 'dbAffiliate_Analytics'
BEGIN
    RAISERROR('This script must be run against the dbAffiliate_Analytics database', 16, 1);
    RETURN;
END

-- Create PixelEvents table for analytics tracking
CREATE TABLE [dbo].[PixelEvents] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [EventType] NVARCHAR(50) NOT NULL,
    [PlayerId] NVARCHAR(100) NOT NULL,
    [BannerTag] NVARCHAR(100) NOT NULL,
    [Metadata] NVARCHAR(MAX) NULL, -- JSON string for metadata dictionary
    [SourceIp] NVARCHAR(45) NULL, -- Support IPv6 addresses
    [UserAgent] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    -- Constraints
    CONSTRAINT [CK_PixelEvents_EventType] CHECK ([EventType] IN ('visit', 'registration', 'deposit', 'conversion')),
    CONSTRAINT [CK_PixelEvents_CreatedAt] CHECK ([CreatedAt] <= GETUTCDATE())
);

-- Add table description
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Stores raw pixel tracking events for campaign analytics. High-volume table for real-time user interaction tracking.',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents';

-- Add column descriptions
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Unique identifier for the pixel event',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Type of event: visit, registration, deposit, or conversion',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'EventType';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Unique identifier for the player/user',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'PlayerId';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Campaign banner/tag identifier for tracking marketing performance',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'BannerTag';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'JSON metadata containing additional tracking parameters',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'Metadata';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'Source IP address of the user (supports IPv4 and IPv6)',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'SourceIp';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'User agent string from the browser',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'UserAgent';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', @value = N'UTC timestamp when the event was created',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'PixelEvents',
    @level2type = N'COLUMN', @level2name = N'CreatedAt';

PRINT 'PixelEvents table created successfully in dbAffiliate_Analytics';
PRINT 'Table supports: visit, registration, deposit, and conversion events';
PRINT 'Ready for Campaign Analytics and Player Tracking';