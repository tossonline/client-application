/* Copyright (c) DigiOutsource. All rights reserved. */

CREATE OR ALTER PROC dbo.usp_AnalyticsArchive @RetentionPeriodDays INT = 90,
                                                       @BatchSize INT = 10000,
                                                       @BatchDelay VARCHAR(8) = '00:00:05'
AS
BEGIN

    DECLARE @AllRowsArchived BIT = 0,
        @RetentionDate DATE = DATEADD(DAY, -@RetentionPeriodDays, GETUTCDATE());

    BEGIN TRY

        WHILE @AllRowsArchived = 0
            BEGIN

                BEGIN TRANSACTION;

                --======================================================================
                --Add tables to move to the archive database here
                --ensure that you use the batch size when doing the delete statements

                -- DELETE TOP @BatchSize
                -- OUTPUT Columns
                -- INTO DestinationArchiveDatbaseTable
                -- FROM SourceTable
                -- WHERE SomeDate < @RetentionDate
                --======================================================================

                IF (@@ROWCOUNT = 0)
                    BEGIN
                        SELECT @AllRowsArchived = 1
                    END;

                COMMIT TRANSACTION;

                WAITFOR DELAY @BatchDelay;

            END;

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            AND @@TranCount > 0
            BEGIN
                ROLLBACK TRANSACTION
            END;

        THROW
    END CATCH;
END;