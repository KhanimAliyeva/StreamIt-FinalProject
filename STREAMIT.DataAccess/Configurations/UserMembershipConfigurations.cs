using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class UserMembershipConfiguration : IEntityTypeConfiguration<UserMembership>
    {
        public void Configure(EntityTypeBuilder<UserMembership> builder)
        {
            builder.ToTable("UserMemberships");

            builder.HasKey(um => um.Id);

            // UserId
            builder.Property(um => um.UserId)
                   .IsRequired();

            // Dates
            builder.Property(um => um.StartDate)
                   .IsRequired();

            builder.Property(um => um.EndDate)
                   .IsRequired();

            // Payment
            builder.Property(um => um.PaidAmount)
                   .HasPrecision(8, 2)
                   .IsRequired();

            builder.Property(um => um.PaymentMethod)
                   .HasMaxLength(50)
                   .IsRequired();

            // Flags
            builder.Property(um => um.IsActive)
                   .HasDefaultValue(true);

            builder.Property(um => um.IsTrial)
                   .HasDefaultValue(false);

            builder.Property(um => um.AutoRenew)
                   .HasDefaultValue(false);

            // User → UserMembership (1-to-many)
            builder.HasOne(um => um.User)
                   .WithMany(u => u.UserMemberships)
                   .HasForeignKey(um => um.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Membership → UserMembership (1-to-many)
            builder.HasOne(um => um.Membership)
                   .WithMany(m => m.UserMemberships)
                   .HasForeignKey(um => um.MembershipId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1 user → eyni anda yalnız 1 aktiv membership
            builder.HasIndex(um => new { um.UserId, um.IsActive })
                   .HasFilter("[IsActive] = 1");
        }
    }

}
