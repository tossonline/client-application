/* Copyright (c) DigiOutsource. All rights reserved. */

CREATE OR ALTER PROC dbo.usp_CleanupArchive @RetentionPeriodDays INT = 6,
                                            @BatchSize INT = 100000,
                                            @BatchDelay VARCHAR(8) = '00:00:02'
AS

BEGIN
    BEGIN TRY
        DECLARE @AllRowsArchive BIT = 0,
            @RetentionDate DATE = DATEADD(DAY, -@RetentionPeriodDays, GETUTCDATE());

        SET @AllRowsArchive = 0;
        WHILE @AllRowsArchive = 0
            BEGIN

                BEGIN TRANSACTION;

                --======================================================================
                --Add tables to permanently delete here
                --ensure that you use the batch size when doing the delete statements

                -- DELETE TOP @BatchSize
                -- WHERE SomeDate < @RetentionDate
                --======================================================================

                IF (@@ROWCOUNT = 0)
                    BEGIN
                        SELECT @AllRowsArchive = 1
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
