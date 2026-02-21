using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.MovieDtos
{
    public class CreateMovieDto
    {
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public IFormFile Poster { get; set; } = null!;
        public IFormFile Trailer { get; set; }=null!;
        public IFormFile Movie { get; set; } = null!;

        public DateTime ReleaseDate { get; set; }
        public float Duration { get; set; }

        public int MembershipId { get; set; }
        public int LanguageId { get; set; }

        public decimal Imdb { get; set; }

        public List<int> GenreIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
        public List<int> PersonIds { get; set; } = new();

    }
}
