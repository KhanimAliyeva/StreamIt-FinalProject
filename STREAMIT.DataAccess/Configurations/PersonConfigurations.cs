using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STREAMIT.Core.Entities;

namespace STREAMIT.DataAccess.Configurations
{

    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(p => p.ImageUrl)
                   .HasMaxLength(500);

            builder.Property(p => p.Biography)
                   .HasMaxLength(4000);

            builder.Property(p => p.Gender)
                   .HasMaxLength(20);

            builder.Property(p => p.Popularity)
                   .IsRequired();

           
            
        }
    }

}
