-- =============================================
-- Get Campaign Performance
-- Purpose: Retrieve campaign performance for a date range
-- =============================================

CREATE PROCEDURE [dbo].[sp_GetCampaignPerformance]
    @BannerTag NVARCHAR(100),
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
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
        WHERE BannerTag = @BannerTag
          AND EventDate BETWEEN @FromDate AND @ToDate
        ORDER BY EventDate;
        
        -- Return summary metrics
        SELECT 
            @BannerTag AS BannerTag,
            @FromDate AS FromDate,
            @ToDate AS ToDate,
            SUM(Visits) AS TotalVisits,
            SUM(Registrations) AS TotalRegistrations,
            SUM(Deposits) AS TotalDeposits,
            CASE 
                WHEN SUM(Visits) > 0 
                THEN CAST(SUM(Deposits) AS DECIMAL(10,4)) / SUM(Visits)
                ELSE 0.0000
            END AS OverallConversionRate,
            CASE 
                WHEN SUM(Visits) > 0 
                THEN CAST(SUM(Registrations) AS DECIMAL(10,4)) / SUM(Visits)
                ELSE 0.0000
            END AS OverallRegistrationRate,
            CASE 
                WHEN SUM(Registrations) > 0 
                THEN CAST(SUM(Deposits) AS DECIMAL(10,4)) / SUM(Registrations)
                ELSE 0.0000
            END AS OverallDepositRate
        FROM [dbo].[vw_CampaignPerformanceSummary]
        WHERE BannerTag = @BannerTag
          AND EventDate BETWEEN @FromDate AND @ToDate;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;