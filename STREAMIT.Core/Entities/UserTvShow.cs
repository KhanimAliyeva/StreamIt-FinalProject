using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class UserTvShow:BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int TvShowId { get; set; }


        public AppUser? User { get; set; }
        public TVShow? TVShow { get; set; }
    }
}
