using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class TvShowPersonConfiguration : IEntityTypeConfiguration<TvShowPerson>
    {
        public void Configure(EntityTypeBuilder<TvShowPerson> builder)
        {
            builder.ToTable("TvShowPeople");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.CastOrder)
                   .IsRequired();

            // TVShow → TvShowPerson
            builder.HasOne(x => x.TvShow)
                   .WithMany(t => t.TvShowPeople)
                   .HasForeignKey(x => x.TvShowId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Person → TvShowPerson
            builder.HasOne(x => x.Person)
                   .WithMany(p => p.TVShowPeople)
                   .HasForeignKey(x => x.PersonId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Eyni person eyni TVShow-da 1 dəfə olsun
            builder.HasIndex(x => new { x.TvShowId, x.PersonId })
                   .IsUnique();

            // Cast sırası üçün index (opsional, amma çox faydalı)
            builder.HasIndex(x => new { x.TvShowId, x.CastOrder });
        }
    }

}
