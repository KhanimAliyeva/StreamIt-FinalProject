using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages");

            // Primary Key
            builder.HasKey(l => l.Id);

            // Properties
            builder.Property(l => l.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(l => l.Code)
                   .IsRequired()
                   .HasMaxLength(10);

            // Relationships

            // Language -> Movies (One to Many)
            builder.HasMany(l => l.Movies)
                   .WithOne(m => m.Language)
                   .HasForeignKey(m => m.LanguageId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Language -> TVShows (One to Many)
            builder.HasMany(l => l.TVShows)
                   .WithOne(t => t.Language)
                   .HasForeignKey(t => t.LanguageId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Language code unikaldır (en, az, tr və s.)
            builder.HasIndex(l => l.Code)
                   .IsUnique();

            builder.HasIndex(l => l.Name);
        }
    }
}
