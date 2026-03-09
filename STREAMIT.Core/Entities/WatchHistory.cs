using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class WatchHistory
    {
        public int Id { get; set; }

        public string AppUserId { get; set; }

        public int MovieId { get; set; }

        public int WatchedMinutes { get; set; }

        public int Duration { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime LastWatchedAt { get; set; }

        public AppUser? AppUser { get; set; }
        public Movie? Movie { get; set; }
    }
}
