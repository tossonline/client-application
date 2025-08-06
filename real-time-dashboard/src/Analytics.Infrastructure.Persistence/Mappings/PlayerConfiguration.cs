// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Persistence.Mappings
{
    public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Players");
            
            builder.HasKey(p => p.PlayerId);
            
            builder.Property(p => p.PlayerId)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.Property(p => p.FirstSeen)
                .IsRequired();
                
            builder.Property(p => p.LastEventAt);
            
            builder.Property(p => p.RegistrationAt);
            
            builder.Property(p => p.DepositAt);
            
            builder.Property(p => p.TotalDeposits)
                .HasDefaultValue(0);

            // Add audit fields
            builder.Property<DateTime>("CreatedAt")
                .HasDefaultValueSql("GETUTCDATE()");
                
            builder.Property<DateTime>("UpdatedAt")
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes for performance
            builder.HasIndex(p => p.FirstSeen)
                .HasDatabaseName("IX_Players_FirstSeen");
                
            builder.HasIndex(p => p.RegistrationAt)
                .HasDatabaseName("IX_Players_RegistrationAt");
                
            builder.HasIndex(p => p.DepositAt)
                .HasDatabaseName("IX_Players_DepositAt");
                
            builder.HasIndex(p => p.TotalDeposits)
                .HasDatabaseName("IX_Players_TotalDeposits");
        }
    }
}