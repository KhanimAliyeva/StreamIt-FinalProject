using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class MoviePerson:BaseEntity
    {
        public int MovieId { get; set; }
        public int PersonId { get; set; }
        public string Role { get; set; } = string.Empty;
        public int CastOrder { get; set; }

        public Movie? Movie { get; set; }
        public Person? Person { get; set; }
    }
}
