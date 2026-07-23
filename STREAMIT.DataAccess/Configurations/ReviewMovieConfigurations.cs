using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    internal class ReviewMovieConfigurations : IEntityTypeConfiguration<ReviewMovie>
    {
        public void Configure(EntityTypeBuilder<ReviewMovie> builder)
        {

            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Rating)
                   .IsRequired()
                   .HasPrecision(2, 1); // 0.0 - 9.9 kimi decimal

            builder.Property(r => r.Comment)
                   .IsRequired()
                   .HasMaxLength(2000);

            // Relationships

            // Review -> Movie
            builder.HasOne(r => r.Movie)
                   .WithMany(m => m.Reviews)
                   .HasForeignKey(r => r.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Review -> User
            builder.HasOne(r => r.User)
                   .WithMany(u => u.MovieReviews)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

           
            builder.HasIndex(r => new { r.MovieId, r.UserId })
                   .IsUnique()
                   .HasFilter("\"ParentReviewId\" IS NULL");

            // Self-referencing: Review -> Replies
            builder.HasOne(r => r.ParentReview)
                   .WithMany(r => r.Replies)
                   .HasForeignKey(r => r.ParentReviewId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Tez-tez sort / filter üçün index
            builder.HasIndex(r => r.Rating);
            builder.HasIndex(r => r.CreatedDate);
        }
    }
}

