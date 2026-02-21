using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class TvShowTagConfiguration : IEntityTypeConfiguration<TvShowTag>
    {
        public void Configure(EntityTypeBuilder<TvShowTag> builder)
        {
            builder.ToTable("TvShowTags");

            builder.HasKey(x => x.Id);

            // TVShow → TvShowTag
            builder.HasOne(x => x.TvShow)
                   .WithMany(t => t.TvShowTags)
                   .HasForeignKey(x => x.TvShowId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Tag → TvShowTag
            builder.HasOne(x => x.Tag)
                   .WithMany(t => t.TVShowTags)
                   .HasForeignKey(x => x.TagId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Eyni tag eyni TVShow-a 2 dəfə bağlanmasın
            builder.HasIndex(x => new { x.TvShowId, x.TagId })
                   .IsUnique();
        }
    }

}
