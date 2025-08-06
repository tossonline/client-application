-- =============================================
-- Player Funnel Analysis View
-- Purpose: Player conversion funnel analysis
-- =============================================

CREATE VIEW [dbo].[vw_PlayerFunnelAnalysis]
AS
SELECT 
    p.PlayerId,
    p.FirstSeen,
    p.RegistrationAt,
    p.DepositAt,
    p.TotalDeposits,
    CASE WHEN p.RegistrationAt IS NOT NULL THEN 1 ELSE 0 END AS IsRegistered,
    CASE WHEN p.DepositAt IS NOT NULL THEN 1 ELSE 0 END AS HasDeposited,
    CASE 
        WHEN p.RegistrationAt IS NOT NULL 
        THEN DATEDIFF(day, p.FirstSeen, p.RegistrationAt) 
        ELSE NULL 
    END AS DaysToRegistration,
    CASE 
        WHEN p.DepositAt IS NOT NULL AND p.RegistrationAt IS NOT NULL
        THEN DATEDIFF(day, p.RegistrationAt, p.DepositAt) 
        ELSE NULL 
    END AS DaysToFirstDeposit,
    CASE 
        WHEN p.DepositAt IS NOT NULL 
        THEN DATEDIFF(day, p.FirstSeen, p.DepositAt) 
        ELSE NULL 
    END AS DaysToFirstDepositFromFirstSeen,
    CASE 
        WHEN p.LastEventAt IS NOT NULL 
        THEN DATEDIFF(day, p.LastEventAt, GETUTCDATE()) 
        ELSE NULL 
    END AS DaysSinceLastActivity
FROM [dbo].[Players] p;