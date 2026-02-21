using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;

namespace STREAMIT.DataAccess.Configurations;


public class MoviePersonConfiguration : IEntityTypeConfiguration<MoviePerson>
{
    public void Configure(EntityTypeBuilder<MoviePerson> builder)
    {

        // Primary Key
        builder.HasKey(mp => mp.Id);

        // Properties
        builder.Property(mp => mp.Role)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(mp => mp.CastOrder)
               .IsRequired();

        // Relationships

        // MoviePerson -> Movie (Many to One)
        builder.HasOne(mp => mp.Movie)
               .WithMany(m => m.MoviePeople)
               .HasForeignKey(mp => mp.MovieId)
               .OnDelete(DeleteBehavior.Cascade);

        // MoviePerson -> Person (Many to One)
        builder.HasOne(mp => mp.Person)
               .WithMany(p => p.MoviePeople)
               .HasForeignKey(mp => mp.PersonId)
               .OnDelete(DeleteBehavior.Restrict);

        // Constraints / Indexes

        // Eyni person eyni movie-də bir dəfə olsun
        builder.HasIndex(mp => new { mp.MovieId, mp.PersonId })
               .IsUnique();

        // Cast order üzrə sıralama üçün
        builder.HasIndex(mp => new { mp.MovieId, mp.CastOrder });
    }
}

