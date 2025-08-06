-- =============================================
-- Upsert Event Summary
-- Purpose: Insert or update daily event summaries
-- Database: dbAffiliate_Analytics
-- Script Type: sprocs - recreated on each run
-- Used by: MetricsAggregate for event aggregation
-- =============================================

CREATE PROCEDURE [dbo].[sp_UpsertEventSummary]
    @EventDate DATE,
    @EventType NVARCHAR(50),
    @BannerTag NVARCHAR(100),
    @Count INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        MERGE [dbo].[EventSummaries] AS target
        USING (SELECT @EventDate AS EventDate, @EventType AS EventType, @BannerTag AS BannerTag, @Count AS Count) AS source
        ON target.EventDate = source.EventDate 
           AND target.EventType = source.EventType 
           AND target.BannerTag = source.BannerTag
        WHEN MATCHED THEN
            UPDATE SET 
                Count = target.Count + source.Count, -- Increment existing count
                UpdatedAt = GETUTCDATE()
        WHEN NOT MATCHED THEN
            INSERT (EventDate, EventType, BannerTag, Count, CreatedAt, UpdatedAt)
            VALUES (source.EventDate, source.EventType, source.BannerTag, source.Count, GETUTCDATE(), GETUTCDATE());
        
        -- Return success
        RETURN 0;
    END TRY
    BEGIN CATCH
        -- Log error and return error code
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        RETURN -1;
    END CATCH
END;