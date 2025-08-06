// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Application.Models.DeadLetter
{
    [Serializable]
    public sealed record DeadLetterCountMetric
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