using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class UserMovieConfiguration : IEntityTypeConfiguration<UserMovie>
    {
        public void Configure(EntityTypeBuilder<UserMovie> builder)
        {
            builder.ToTable("UserMovies");

            builder.HasKey(um => um.Id);

            builder.Property(um => um.Type)
                   .IsRequired()
                   .HasMaxLength(50);

            // User → UserMovie (1-to-many)
            builder.HasOne(um => um.User)
                   .WithMany(u => u.UserMovies)
                   .HasForeignKey(um => um.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Movie → UserMovie (1-to-many)
            builder.HasOne(um => um.Movie)
                   .WithMany(m => m.UserMovies)
                   .HasForeignKey(um => um.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Eyni user eyni movie-ni eyni type ilə 1 dəfə edə bilər
            builder.HasIndex(um => new { um.UserId, um.MovieId, um.Type })
                   .IsUnique();
        }
    }

}
