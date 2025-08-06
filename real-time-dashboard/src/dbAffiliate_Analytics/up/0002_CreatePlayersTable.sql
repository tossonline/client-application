-- =============================================
-- Create Players Table
-- Purpose: Store player lifecycle and tracking information
-- =============================================

CREATE TABLE [dbo].[Players] (
    [PlayerId] NVARCHAR(100) PRIMARY KEY,
    [FirstSeen] DATETIME2(7) NOT NULL,
    [LastEventAt] DATETIME2(7) NULL,
    [RegistrationAt] DATETIME2(7) NULL,
    [DepositAt] DATETIME2(7) NULL,
    [TotalDeposits] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
);

PRINT 'Players table created successfully';