// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Persistence.Mappings
{
    public sealed class EventSummaryConfiguration : IEntityTypeConfiguration<EventSummary>
    {
        public void Configure(EntityTypeBuilder<EventSummary> builder)
        {
            builder.ToTable("EventSummaries");
            
            // Use a surrogate key for performance
            builder.HasKey("Id");
            builder.Property<long>("Id")
                .ValueGeneratedOnAdd();
                
            builder.Property(e => e.EventDate)
                .HasColumnType("DATE")
                .IsRequired();
                
            builder.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsRequired();
                
            builder.Property(e => e.BannerTag)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(e => e.Count)
                .HasDefaultValue(0);

            // Add audit fields
            builder.Property<DateTime>("CreatedAt")
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property<DateTime>("UpdatedAt")
                .HasDefaultValueSql("GETUTCDATE()");

            // Unique constraint to prevent duplicate daily summaries
            builder.HasIndex(e => new { e.EventDate, e.EventType, e.BannerTag })
                .IsUnique()
                .HasDatabaseName("UQ_EventSummaries_Date_Type_Banner");

            // Indexes for performance
            builder.HasIndex(e => e.EventDate)
                .HasDatabaseName("IX_EventSummaries_EventDate");
                
            builder.HasIndex(e => e.BannerTag)
                .HasDatabaseName("IX_EventSummaries_BannerTag");
                
            builder.HasIndex(e => e.EventType)
                .HasDatabaseName("IX_EventSummaries_EventType");
                
            builder.HasIndex(e => new { e.BannerTag, e.EventDate })
                .HasDatabaseName("IX_EventSummaries_BannerTag_EventDate");
                
            builder.HasIndex(e => new { e.EventType, e.EventDate })
                .HasDatabaseName("IX_EventSummaries_EventType_EventDate");
        }
    }
}