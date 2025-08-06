// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Analytics.Domain.Entities;
using System.Text.Json;

namespace Analytics.Infrastructure.Persistence.Mappings
{
    public sealed class PixelEventConfiguration : IEntityTypeConfiguration<PixelEvent>
    {
        public void Configure(EntityTypeBuilder<PixelEvent> builder)
        {
            builder.ToTable("PixelEvents");
            
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Id)
                .HasDefaultValueSql("NEWID()");
                
            builder.Property(p => p.EventType)
                .HasMaxLength(50)
                .IsRequired();
                
            builder.Property(p => p.PlayerId)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(p => p.BannerTag)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(p => p.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>())
                .HasColumnType("NVARCHAR(MAX)");
                
            builder.Property(p => p.SourceIp)
                .HasMaxLength(45); // Support IPv6
                
            builder.Property(p => p.UserAgent)
                .HasMaxLength(500);
                
            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes for performance
            builder.HasIndex(p => p.PlayerId)
                .HasDatabaseName("IX_PixelEvents_PlayerId");
                
            builder.HasIndex(p => p.BannerTag)
                .HasDatabaseName("IX_PixelEvents_BannerTag");
                
            builder.HasIndex(p => p.EventType)
                .HasDatabaseName("IX_PixelEvents_EventType");
                
            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_PixelEvents_CreatedAt");
                
            builder.HasIndex(p => new { p.PlayerId, p.EventType })
                .HasDatabaseName("IX_PixelEvents_PlayerId_EventType");
                
            builder.HasIndex(p => new { p.BannerTag, p.CreatedAt })
                .HasDatabaseName("IX_PixelEvents_BannerTag_CreatedAt");
        }
    }
}