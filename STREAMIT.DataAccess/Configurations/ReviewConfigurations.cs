using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;

namespace STREAMIT.DataAccess.Configurations
{
    // Combined configuration for review entities (movies and TV shows).
    // Implements both IEntityTypeConfiguration<ReviewMovie> and IEntityTypeConfiguration<ReviewTvShow>
    internal class CombinedReviewConfigurations : IEntityTypeConfiguration<ReviewMovie>
    {
        public void Configure(EntityTypeBuilder<ReviewMovie> builder)
        {
            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Rating)
                   .IsRequired()
                   .HasPrecision(2, 1);

            builder.Property(r => r.Comment)
                   .IsRequired()
                   .HasMaxLength(2000);

            // Relationships
            builder.HasOne(r => r.Movie)
                   .WithMany(m => m.Reviews)
                   .HasForeignKey(r => r.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                   .WithMany(u => u.MovieReviews)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one review per user per movie
            builder.HasIndex(r => new { r.MovieId, r.UserId }).IsUnique();

            // Helpful indexes
            builder.HasIndex(r => r.Rating);
            builder.HasIndex(r => r.CreatedDate);
        }

    }
}
