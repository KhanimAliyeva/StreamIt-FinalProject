using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {

        // Primary Key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(m => m.Status)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(m => m.Content)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(m => m.PosterUrl)
               .HasMaxLength(500);

        builder.Property(m => m.MovieUrl)
               .HasMaxLength(500);

        builder.Property(m => m.TrailerUrl)
               .HasMaxLength(500);

        builder.Property(m => m.ReleaseDate)
               .IsRequired();

        builder.Property(m => m.Duration)
               .IsRequired();

        builder.Property(m => m.Imdb)
               .HasPrecision(3, 1);

        // Relationships

        //// Movie -> Author (Many to One)
        //builder.HasOne(m => m.MoviePeople)
        //       .WithMany(a => a.m)
        //       .HasForeignKey(m => m.AuthorId)
        //       .HasPrincipalKey(a => a.Id)
        //       .OnDelete(DeleteBehavior.Restrict);

        // Movie -> Membership (Many to One)
        builder.HasOne(m => m.Membership)
               .WithMany(ms => ms.Movies)
               .HasForeignKey(m => m.MembershipId)
               .OnDelete(DeleteBehavior.Restrict);

        // Movie -> Language (Many to One)
        builder.HasOne(m => m.Language)
               .WithMany(l => l.Movies)
               .HasForeignKey(m => m.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);

        // Movie -> MovieStatistics (One to One)
        builder.HasOne(m => m.MovieStatistics)
               .WithOne(ms => ms.Movie)
               .HasForeignKey<MovieStatistics>(ms => ms.MovieId);

        // Movie -> Reviews (One to Many)
        builder.HasMany(m => m.Reviews)
               .WithOne(r => r.Movie)
               .HasForeignKey(r => r.MovieId);

        // Junction tables
        builder.HasMany(m => m.MovieGenres)
               .WithOne(mg => mg.Movie)
               .HasForeignKey(mg => mg.MovieId);

        builder.HasMany(m => m.MovieTags)
               .WithOne(mt => mt.Movie)
               .HasForeignKey(mt => mt.MovieId);

        builder.HasMany(m => m.MoviePeople)
               .WithOne(mp => mp.Movie)
               .HasForeignKey(mp => mp.MovieId)
               .HasPrincipalKey(mp => mp.Id);
        

        // Indexes
        builder.HasIndex(m => m.Title);
        builder.HasIndex(m => m.ReleaseDate);
        builder.HasIndex(m => m.Imdb);


    }
}
