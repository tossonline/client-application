-- =============================================
-- Calculate Campaign Metrics
-- Purpose: Calculate and store campaign metrics
-- Database: dbAffiliate_Analytics
-- Script Type: sprocs - recreated on each run
-- Used by: Background jobs and MetricsAggregate for metric calculation
-- =============================================

CREATE PROCEDURE [dbo].[sp_CalculateCampaignMetrics]
    @BannerTag NVARCHAR(100) = NULL,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @RowsProcessed INT = 0;
        
        -- Insert/Update campaign metrics from the performance view
        WITH CampaignData AS (
            SELECT 
                BannerTag,
                EventDate,
                Visits,
                Registrations,
                Deposits,
                ConversionRate,
                VisitToRegistrationRate,
                RegistrationToDepositRate
            FROM [dbo].[vw_CampaignPerformanceSummary]
            WHERE EventDate BETWEEN @FromDate AND @ToDate
              AND (@BannerTag IS NULL OR BannerTag = @BannerTag)
        )
        MERGE [dbo].[CampaignMetrics] AS target
        USING CampaignData AS source
        ON target.BannerTag = source.BannerTag AND target.MetricDate = source.EventDate
        WHEN MATCHED THEN
            UPDATE SET
                TotalVisits = source.Visits,
                TotalRegistrations = source.Registrations,
                TotalDeposits = source.Deposits,
                ConversionRate = source.ConversionRate,
                VisitToRegistrationRate = source.VisitToRegistrationRate,
                RegistrationToDepositRate = source.RegistrationToDepositRate,
                CalculatedAt = GETUTCDATE()
        WHEN NOT MATCHED THEN
            INSERT (BannerTag, MetricDate, TotalVisits, TotalRegistrations, TotalDeposits, 
                   ConversionRate, VisitToRegistrationRate, RegistrationToDepositRate, CalculatedAt)
            VALUES (source.BannerTag, source.EventDate, source.Visits, source.Registrations, 
                   source.Deposits, source.ConversionRate, source.VisitToRegistrationRate, 
                   source.RegistrationToDepositRate, GETUTCDATE());
        
        SET @RowsProcessed = @@ROWCOUNT;
        
        -- Return processing summary
        SELECT 
            @RowsProcessed AS RowsProcessed,
            @FromDate AS FromDate,
            @ToDate AS ToDate,
            @BannerTag AS BannerTag,
            GETUTCDATE() AS ProcessedAt;
        
        RETURN @RowsProcessed;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        RETURN -1;
    END CATCH
END;