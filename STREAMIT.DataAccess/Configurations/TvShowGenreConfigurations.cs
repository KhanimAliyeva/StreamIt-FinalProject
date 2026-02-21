using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class TvShowGenreConfiguration : IEntityTypeConfiguration<TvShowGenre>
    {
        public void Configure(EntityTypeBuilder<TvShowGenre> builder)
        {
            builder.ToTable("TvShowGenres");

            builder.HasKey(x => x.Id);

            // TVShow → TvShowGenre
            builder.HasOne(x => x.TvShow)
                   .WithMany(t => t.TvShowGenres)
                   .HasForeignKey(x => x.TvShowId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Genre → TvShowGenre
            builder.HasOne(x => x.Genre)
                   .WithMany(g => g.TVShowGenres)
                   .HasForeignKey(x => x.GenreId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.TvShowId, x.GenreId })
                   .IsUnique();
        }
    }

}
