using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.WatchHistoryDtos
{
    public class UpdateWatchHistoryDto
    {
        public int MovieId { get; set; }
        public int WatchedMinutes { get; set; }
        public int Duration { get; set; }
        public string? UserId { get; set; }
    }

    public class ContinueWatchingDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public int WatchedMinutes { get; set; }
        public int Duration { get; set; }
        public int ProgressPercent { get; set; }
    }
}
