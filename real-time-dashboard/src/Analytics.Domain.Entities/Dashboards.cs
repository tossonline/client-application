// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Repository.Abstractions;

namespace Analytics.Domain.Entities
{
    public sealed class Dashboards : IAggregate<string>
    {
        public Dashboards()
        {
            Id = string.Empty;
        }

        public string Id { get; }
    }
}