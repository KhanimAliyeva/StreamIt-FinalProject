using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;

public class MovieStatisticsConfiguration : IEntityTypeConfiguration<MovieStatistics>
{
    public void Configure(EntityTypeBuilder<MovieStatistics> builder)
    {
        builder.ToTable("MovieStatistics");

        // Primary Key
        builder.HasKey(ms => ms.Id);

        // Properties
        builder.Property(ms => ms.ViewCount)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(ms => ms.LikeCount)
               .IsRequired()
               .HasDefaultValue(0);

        // Relationships

        // MovieStatistics -> Movie (One to One)
        builder.HasOne(ms => ms.Movie)
               .WithOne(m => m.MovieStatistics)
               .HasForeignKey<MovieStatistics>(ms => ms.MovieId)
               .OnDelete(DeleteBehavior.Cascade);


        // Hər movie üçün yalnız 1 statistics record
        builder.HasIndex(ms => ms.MovieId)
               .IsUnique();
    }
}

