// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.Configuration.SqlServer;

namespace Analytics.Domain.Models.Configuration.SqlQueueConsumer
{
    [Serializable]
    public sealed class SqlQueueConsumerConfiguration : SqlServerConfiguration
    {
        public SqlQueueConsumerConfiguration()
        {
            Server = string.Empty;
            Database = string.Empty;
            Schema = string.Empty;
            Table = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Encrypt = false;
            AllowMultipleActiveResultSets = true;
        }

        public string Schema { get; init; }

        public string Table { get; init; }

        public int CommandTimeoutSeconds { get; init; }

        public int ConnectionTimeoutSeconds { get; init; }
    }
}