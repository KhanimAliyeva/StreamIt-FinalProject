using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class MovieGenre:BaseEntity
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }

        public Movie? Movie { get; set; }
        public Genre? Genre { get; set; }
    }
}
