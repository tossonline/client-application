// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Messaging.Abstractions.Messages;

namespace Analytics.Application.Models.DeadLetter
{
    [Serializable]
    public sealed record DeadLetterCountMetric : Message
    {
        public DeadLetterCountMetric()
        {
            Name = string.Empty;
        }

        public DeadLetterCountMetric(string name, int recordCount)
        {
            Name = name;
            RecordCount = recordCount;
        }

        public string Name { get; init; }

        public int RecordCount { get; init; }
    }
}