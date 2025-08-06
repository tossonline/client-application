/* Copyright (c) DigiOutsource. All rights reserved. */

CREATE OR ALTER PROC Grafana.usp_DeadLetterCountGet
AS
BEGIN

    SELECT t.name      Name,
           MAX(p.rows) RecordCount
    FROM sys.tables t
             INNER JOIN sys.indexes i
                        ON t.object_id = i.object_id
             INNER JOIN sys.partitions p
                        ON i.object_id = p.object_id
                            AND i.index_id = p.index_id
    WHERE t.name LIKE 'deadletter%'
    GROUP BY t.name

END;