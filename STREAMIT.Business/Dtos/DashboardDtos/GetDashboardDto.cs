using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.DashboardDtos
{
    public class GetDashboardDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalSubscribers { get; set; }
        public int TotalMovies { get; set; }
        public int TotalReviews { get; set; }
        public int TotalFavorites { get; set; }
        public int NewUsersThisMonth { get; set; }

        public List<string> GenreLabels { get; set; } = new();
        public List<int> GenreCounts { get; set; } = new();

        public List<string> UserLabels { get; set; } = new();
        public List<int> UserCounts { get; set; } = new();
    }
}
