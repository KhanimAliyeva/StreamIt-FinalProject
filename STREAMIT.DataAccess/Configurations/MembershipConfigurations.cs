using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.ToTable("Memberships");

            // Primary Key
            builder.HasKey(m => m.Id);

            // Properties
            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.Price)
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(m => m.DurationInDays)
                   .IsRequired();

            builder.Property(m => m.VideoQuality)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(m => m.MaxDevices)
                   .IsRequired();

            builder.Property(m => m.HasAds)
                   .IsRequired();

            builder.Property(m => m.CanDownload)
                   .IsRequired();

            builder.Property(m => m.IsActive)
                   .IsRequired();

            builder.Property(m => m.PriorityLevel)
                   .IsRequired();

            // Relationships

            // Membership -> Movies (One to Many)
            builder.HasMany(m => m.Movies)
                   .WithOne(movie => movie.Membership)
                   .HasForeignKey(movie => movie.MembershipId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Membership -> UserMemberships (One to Many)
            builder.HasMany(m => m.UserMemberships)
                   .WithOne(um => um.Membership)
                   .HasForeignKey(um => um.MembershipId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Constraints / Indexes

            // Plan adı unikaldır (Basic, Standard, Premium)
            builder.HasIndex(m => m.Name)
                   .IsUnique();

            // Access logic üçün tez-tez istifadə olunacaq
            builder.HasIndex(m => m.PriorityLevel);

            // Aktiv planların filteri üçün
            builder.HasIndex(m => m.IsActive);
        }
    }
}
