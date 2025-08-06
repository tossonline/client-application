// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Observability.Messages
{
    public static class ErrorMessages
    {
        public const string ENTITY_FRAMEWORK_UNIQUE_CONSTRAINT_ERROR = "A Unique constraint has been violated";
        public const string ENTITY_FRAMEWORK_UNIQUE_ROW_KEY_ERROR = "A row unique key has been duplicated";
        public const string ENTITY_FRAMEWORK_SAVE_CHANGES_ERROR = "Something went wrong while trying to save the database changes";
        public const string UNIT_OF_WORK_CANNOT_BE_NULL = "_contextFactory cannot be null";
        public const string DAPPER_TIMEOUT_ERROR = "DapperExtensions experienced a SQL timeout";
        public const string DAPPER_SQL_EXCEPTION_ERROR = "DapperExtensions experienced a SQL exception (not a timeout)";
        public const string DAPPER_NON_SQL_EXCEPTION_ERROR = "DapperExtensions experienced a non-SQL exception";
        public const string BACKGROUND_JOB_CONFIGURATION_NOT_FOUND = "Configuration for the specified background job could not be determined";
        public const string BACKGROUND_JOB_SCHEDULE_NOT_FOUND = "Schedule for the given background job could not be determined";
        public const string BACKGROUND_JOB_ARCHIVE_ERROR = "Something went wrong while trying to archive data";
        public const string BACKGROUND_JOB_ARCHIVE_CLEANUP_ERROR = "Something went wrong while trying to cleanup the archive database";
        public const string BACKGROUND_JOB_DEAD_LETTER_METRICS_FETCH_ERROR = "Something went wrong while trying to fetch the deadletter metrics";
    }
}