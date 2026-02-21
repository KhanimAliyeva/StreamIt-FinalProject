using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class MovieStatistics:BaseEntity
    {
        public int MovieId { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }

        public Movie? Movie { get; set; }
    }

}
