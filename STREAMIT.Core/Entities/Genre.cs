using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Genre:BaseEntity
    {
        public string Name { get; set; }=string.Empty;

        public ICollection<MovieGenre> MovieGenres { get; set; } = [];
        public ICollection<TvShowGenre> TVShowGenres { get; set; } = [];
    }
}
