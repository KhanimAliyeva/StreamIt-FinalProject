using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
    {
        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            builder.ToTable("Episodes");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.EpisodeNumber)
                   .IsRequired();

            builder.Property(e => e.Duration)
                   .IsRequired();

            builder.Property(e => e.VideoUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            // Relationships

            // Episode -> Season (Many to One)
            builder.HasOne(e => e.Season)
                   .WithMany(s => s.Episodes)
                   .HasForeignKey(e => e.SeasonId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Constraints

            // Eyni season daxilində eyni episode number olmasın
            builder.HasIndex(e => new { e.SeasonId, e.EpisodeNumber })
                   .IsUnique();

            // Indexes
            builder.HasIndex(e => e.Title);
        }
    }
}
