namespace STREAMIT.MVC.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
       
            public int TotalUsers { get; set; }
            public int ActiveUsers { get; set; }
            public int TotalSubscribers { get; set; }
            public int TotalMovies { get; set; }
            public int TotalReviews { get; set; }
            public int TotalFavorites { get; set; }

            public decimal MonthlyRevenue { get; set; }
            public int NewUsersThisMonth { get; set; }

            public List<string> GenreLabels { get; set; } = new();
            public List<int> GenreCounts { get; set; } = new();

            public List<string> RevenueLabels { get; set; } = new();
            public List<decimal> RevenueData { get; set; } = new();

            public List<string> UserLabels { get; set; } = new();
            public List<int> UserData { get; set; } = new();

            public List<TopMovieItemViewModel> TopMovies { get; set; } = new();
        }

        public class TopMovieItemViewModel
        {
            public string Title { get; set; } = string.Empty;
            public string PosterUrl { get; set; } = string.Empty;
            public int ViewCount { get; set; }
            public double Rating { get; set; }
        }
    }
