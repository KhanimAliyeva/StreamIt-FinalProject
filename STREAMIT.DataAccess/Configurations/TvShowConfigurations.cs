using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations;

public class TVShowConfiguration : IEntityTypeConfiguration<TVShow>
{
    public void Configure(EntityTypeBuilder<TVShow> builder)
    {
        builder.ToTable("TVShows");

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(t => t.Status)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(t => t.Content)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(t => t.PosterUrl)
               .HasMaxLength(500);

        builder.Property(t => t.TrailerUrl)
               .HasMaxLength(500);

        builder.Property(t => t.ReleaseDate)
               .IsRequired();

        builder.Property(t => t.Imdb)
               .HasPrecision(3, 1); 

        // Relationships

       

        // TVShow -> Membership (Many to One)
        builder.HasOne(t => t.Membership)
               .WithMany(m => m.TVShows)
               .HasForeignKey(t => t.MembershipId)
               .OnDelete(DeleteBehavior.Restrict);

        // TVShow -> Language (Many to One)
        builder.HasOne(t => t.Language)
               .WithMany(l => l.TVShows)
               .HasForeignKey(t => t.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);

        // TVShow -> Seasons (One to Many)
        builder.HasMany(t => t.Seasons)
               .WithOne(s => s.TVShow)
               .HasForeignKey(s => s.TVShowId)
               .OnDelete(DeleteBehavior.Cascade);

        // TVShow -> Statistics (One to One)
        builder.HasOne(t => t.TvShowStatistics)
               .WithOne(s => s.TVShow)
               .HasForeignKey<TvShowStatistics>(s => s.TvShowId);

        // TVShow -> Reviews (One to Many)
        builder.HasMany(t => t.Reviews)
               .WithOne(r => r.TvShow)
               .HasForeignKey(r => r.TvShowId);

        // Junction tables
        builder.HasMany(t => t.TvShowGenres)
               .WithOne(g => g.TvShow)
               .HasForeignKey(g => g.TvShowId);

        builder.HasMany(t => t.TvShowTags)
               .WithOne(tg => tg.TvShow)
               .HasForeignKey(tg => tg.TvShowId);

        builder.HasMany(t => t.TvShowPeople)
               .WithOne(p => p.TvShow)
               .HasForeignKey(p => p.TvShowId);

        // Indexes
        builder.HasIndex(t => t.Title);
        builder.HasIndex(t => t.ReleaseDate);
        builder.HasIndex(t => t.Imdb);
    }
}

