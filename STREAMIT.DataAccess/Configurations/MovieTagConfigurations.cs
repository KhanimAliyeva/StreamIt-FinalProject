using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Configurations
{
    public class MovieTagConfigurations : IEntityTypeConfiguration<MovieTag>
    {
        public void Configure(EntityTypeBuilder<MovieTag> builder)
        {
            builder.HasOne(x => x.Movie)
                   .WithMany(x => x.MovieTags)
                   .HasForeignKey(x => x.MovieId);

            builder.HasOne(x => x.Tag)
                   .WithMany(x => x.MovieTags)
                   .HasForeignKey(x => x.TagId);
        }
    }
}
