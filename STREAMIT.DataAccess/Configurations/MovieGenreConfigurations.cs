using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
    {
        public void Configure(EntityTypeBuilder<MovieGenre> builder)
        {
            builder.ToTable("MovieGenres");

            // Composite Primary Key
            builder.HasKey(mg => new { mg.MovieId, mg.GenreId });

            // Relationships

            // MovieGenre -> Movie
            builder.HasOne(mg => mg.Movie)
                   .WithMany(m => m.MovieGenres)
                   .HasForeignKey(mg => mg.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            // MovieGenre -> Genre
            builder.HasOne(mg => mg.Genre)
                   .WithMany(g => g.MovieGenres)
                   .HasForeignKey(mg => mg.GenreId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes (performance üçün)
            builder.HasIndex(mg => mg.MovieId);
            builder.HasIndex(mg => mg.GenreId);
        }
    }
}
