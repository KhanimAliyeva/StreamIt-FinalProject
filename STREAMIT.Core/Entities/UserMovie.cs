using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class UserMovie:BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public string Type { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        public Movie? Movie { get; set; }
    }

}
