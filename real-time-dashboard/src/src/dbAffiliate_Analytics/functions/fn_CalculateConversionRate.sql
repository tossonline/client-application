-- =============================================
-- Calculate Conversion Rate Function
-- Purpose: Calculate conversion rate with null handling
-- Database: dbAffiliate_Analytics
-- Script Type: functions - recreated on each run
-- Used by: All analytics aggregates for conversion calculations
-- =============================================

CREATE FUNCTION [dbo].[fn_CalculateConversionRate]
(
    @Conversions INT,
    @TotalEvents INT
)
RETURNS DECIMAL(5,4)
AS
BEGIN
    DECLARE @ConversionRate DECIMAL(5,4);
    
    IF @TotalEvents IS NULL OR @TotalEvents = 0 OR @Conversions IS NULL
    BEGIN
        SET @ConversionRate = 0.0000;
    END
    ELSE
    BEGIN
        SET @ConversionRate = CAST(@Conversions AS DECIMAL(10,4)) / @TotalEvents;
        
        -- Ensure the result doesn't exceed 1.0000 (100%)
        IF @ConversionRate > 1.0000
            SET @ConversionRate = 1.0000;
    END
    
    RETURN @ConversionRate;
END;