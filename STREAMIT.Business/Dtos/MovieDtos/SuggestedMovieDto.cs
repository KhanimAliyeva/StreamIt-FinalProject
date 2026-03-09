using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.MovieDtos
{
    public class SuggestedMovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public decimal? Imdb { get; set; }
        public string? Slug { get; set; }
    }
}
