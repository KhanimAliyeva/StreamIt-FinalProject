using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class WatchList
    {
        public int Id { get; set; }

        public string UserId { get; set; }=string.Empty;
        public AppUser User { get; set; } = null!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(4);
    }
}
