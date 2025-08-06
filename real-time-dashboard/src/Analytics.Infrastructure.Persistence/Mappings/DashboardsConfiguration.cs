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
            builder.ToTable("Dashboards");
            
            builder.HasKey(d => d.Id);
            
            builder.Property(d => d.Id)
                .HasMaxLength(100)
                .IsRequired();
                
            // Add additional properties for extended Dashboards entity
            builder.Property<string>("Name")
                .HasMaxLength(200)
                .IsRequired();
                
            builder.Property<string>("Description")
                .HasMaxLength(500);
                
            builder.Property<string>("Configuration")
                .HasColumnType("NVARCHAR(MAX)");
                
            builder.Property<bool>("IsActive")
                .HasDefaultValue(true);
                
            builder.Property<DateTime>("CreatedAt")
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property<DateTime>("UpdatedAt")
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property<string>("CreatedBy")
                .HasMaxLength(100);
                
            builder.Property<string>("UpdatedBy")
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex("Name")
                .HasDatabaseName("IX_Dashboards_Name");
                
            builder.HasIndex("IsActive")
                .HasDatabaseName("IX_Dashboards_IsActive");
                
            builder.HasIndex("CreatedAt")
                .HasDatabaseName("IX_Dashboards_CreatedAt");
        }
    }
}