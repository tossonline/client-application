// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.SqlQueueConsumer
{
    [Serializable]
    public sealed class SqlQueueWithDeadletterConfiguration
    {
        public SqlQueueWithDeadletterConfiguration()
        {
            Queue = new SqlQueueConsumerConfiguration();
            DeadLetter = new SqlQueueConsumerConfiguration();

            ObservabilityTag = string.Empty;
        }

        public SqlQueueConsumerConfiguration Queue { get; init; }

        public SqlQueueConsumerConfiguration DeadLetter { get; init; }

        public int NoRecordBackOffWait { get; init; }

        public string ObservabilityTag { get; init; }
    }
}