using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class UserTvShowConfiguration : IEntityTypeConfiguration<UserTvShow>
    {
        public void Configure(EntityTypeBuilder<UserTvShow> builder)
        {
            builder.ToTable("UserTvShows");

            builder.HasKey(ut => ut.Id);

            // User → UserTvShow (1-to-many)
            builder.HasOne(ut => ut.User)
                   .WithMany(u => u.UserTvShows)
                   .HasForeignKey(ut => ut.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // TVShow → UserTvShow (1-to-many)
            builder.HasOne(ut => ut.TVShow)
                   .WithMany(t => t.UserTvShows)
                   .HasForeignKey(ut => ut.TvShowId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Eyni user eyni TVShow-u 1 dəfə əlavə edə bilər
            builder.HasIndex(ut => new { ut.UserId, ut.TvShowId })
                   .IsUnique();
        }
    }

}
