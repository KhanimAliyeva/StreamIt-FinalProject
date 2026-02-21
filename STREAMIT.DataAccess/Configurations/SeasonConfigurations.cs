using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class SeasonConfiguration : IEntityTypeConfiguration<Season>
    {
        public void Configure(EntityTypeBuilder<Season> builder)
        {
            builder.ToTable("Seasons");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SeasonNumber)
                   .IsRequired();

            // TVShow → Season (1-to-many)
            builder.HasOne(s => s.TVShow)
                   .WithMany(t => t.Seasons)
                   .HasForeignKey(s => s.TVShowId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1 TVShow-da eyni season number təkrarlanmasın
            builder.HasIndex(s => new { s.TVShowId, s.SeasonNumber })
                   .IsUnique();
        }
    }

}
