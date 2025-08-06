-- =============================================
-- Get Date Range Function
-- Purpose: Return a table of dates for a given number of days
-- =============================================

CREATE FUNCTION [dbo].[fn_GetDateRange]
(
    @Days INT
)
RETURNS TABLE
AS
RETURN
(
    WITH DateRange AS (
        -- Start with today's date
        SELECT CAST(GETDATE() AS DATE) AS DateValue, 0 AS DayOffset
        
        UNION ALL
        
        -- Recursively add previous days
        SELECT DATEADD(DAY, -1, DateValue), DayOffset + 1
        FROM DateRange
        WHERE DayOffset < @Days - 1
    )
    SELECT 
        DateValue,
        DayOffset,
        DATENAME(WEEKDAY, DateValue) AS DayName,
        CASE 
            WHEN DATEPART(WEEKDAY, DateValue) IN (1, 7) THEN 1 
            ELSE 0 
        END AS IsWeekend,
        CASE 
            WHEN DateValue = CAST(GETDATE() AS DATE) THEN 1 
            ELSE 0 
        END AS IsToday
    FROM DateRange
);