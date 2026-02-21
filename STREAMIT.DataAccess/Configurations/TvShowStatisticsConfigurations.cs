using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class TvShowStatisticsConfiguration : IEntityTypeConfiguration<TvShowStatistics>
    {
        public void Configure(EntityTypeBuilder<TvShowStatistics> builder)
        {
            builder.ToTable("TvShowStatistics");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ViewCount)
                   .HasDefaultValue(0);

            builder.Property(s => s.LikeCount)
                   .HasDefaultValue(0);

            // TVShow ↔ Statistics (1-to-1)
            builder.HasOne(s => s.TVShow)
                   .WithOne(t => t.TvShowStatistics)
                   .HasForeignKey<TvShowStatistics>(s => s.TvShowId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Hər TVShow üçün yalnız 1 statistics olsun
            builder.HasIndex(s => s.TvShowId)
                   .IsUnique();
        }
    }

}
