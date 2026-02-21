using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class TvShowGenre:BaseEntity
    {
        public int TvShowId { get; set; }
        public int GenreId { get; set; }

        public TVShow? TvShow { get; set; }
        public Genre? Genre { get; set; }
    }
}
