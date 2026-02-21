using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class TvShowTag:BaseEntity
    {
        public int TvShowId { get; set; }
        public int TagId { get; set; }

        public TVShow? TvShow { get; set; }
        public Tag? Tag { get; set; }
    }
}
