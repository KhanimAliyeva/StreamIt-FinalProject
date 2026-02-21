using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class TvShowStatistics:BaseEntity
    {

        public int TvShowId { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }

        public TVShow? TVShow { get; set; }

    }
}
