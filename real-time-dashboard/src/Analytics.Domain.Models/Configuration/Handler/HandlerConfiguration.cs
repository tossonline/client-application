// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.Handler
{
    [Serializable]
    public sealed class HandlerConfiguration
    {
        public HandlerConfiguration()
        {
            BatchSize = 0;
            RetryAfterInMilliseconds = 0;
        }

        public int BatchSize { get; set; }

        public int RetryAfterInMilliseconds { get; set; }
    }
}