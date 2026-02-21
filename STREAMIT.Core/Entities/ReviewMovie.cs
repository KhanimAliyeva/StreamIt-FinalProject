using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class ReviewMovie:BaseAuditableEntity
    {
        public int MovieId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Comment { get; set; }=string.Empty;

        public Movie? Movie { get; set; }
        public AppUser? User { get; set; }
    }

}
