using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class ReviewTvShow:BaseAuditableEntity
    {
        public int TvShowId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        public TVShow? TvShow { get; set; }
        public AppUser? User { get; set; }
    }
}
