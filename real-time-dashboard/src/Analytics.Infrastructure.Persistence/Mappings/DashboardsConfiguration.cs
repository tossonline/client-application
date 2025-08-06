// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Persistence.Mappings
{
    public sealed class DashboardsConfiguration : IEntityTypeConfiguration<Dashboards>
    {
        public void Configure(EntityTypeBuilder<Dashboards> builder)
        {
            throw new NotImplementedException();
        }
    }
}