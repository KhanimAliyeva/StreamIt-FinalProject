using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.ToTable("Genres");

            // Primary Key
            builder.HasKey(g => g.Id);

            // Properties
            builder.Property(g => g.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Relationships

            // Genre -> MovieGenres (One to Many)
            builder.HasMany(g => g.MovieGenres)
                   .WithOne(mg => mg.Genre)
                   .HasForeignKey(mg => mg.GenreId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Genre -> TvShowGenres (One to Many)
            builder.HasMany(g => g.TVShowGenres)
                   .WithOne(tg => tg.Genre)
                   .HasForeignKey(tg => tg.GenreId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Constraints / Indexes

            builder.HasIndex(g => g.Name)
                   .IsUnique();
        }
    }
}
