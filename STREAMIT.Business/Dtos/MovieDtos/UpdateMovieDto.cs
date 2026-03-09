using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.MovieDtos
{
    public class UpdateMovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public IFormFile? Poster { get; set; }
        public string YoutubeUrl { get; set; } = string.Empty;
        public IFormFile? Movie { get; set; }

        public DateTime ReleaseDate { get; set; }
        public float Duration { get; set; }

        public int MembershipId { get; set; }
        public int LanguageId { get; set; }


        public List<int> GenreIds { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
        public List<int> PersonIds { get; set; } = new();


    }
}
