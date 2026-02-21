using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class ReviewTvShowConfiguration : IEntityTypeConfiguration<ReviewTvShow>
    {
        public void Configure(EntityTypeBuilder<ReviewTvShow> builder)
        {
            builder.ToTable("ReviewTvShows");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                   .HasPrecision(2, 1)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000);

            // Review → TVShow
            builder.HasOne(r => r.TvShow)
                   .WithMany(t => t.Reviews)
                   .HasForeignKey(r => r.TvShowId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Review → User
            builder.HasOne(r => r.User)
                   .WithMany(u => u.TvShowReviews)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Optional: 1 user 1 show = 1 review
            builder.HasIndex(r => new { r.UserId, r.TvShowId })
                   .IsUnique();
        }
    }

}
