using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.MovieStaticticsDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.TvShowDtos
{
    public class GetTvShowDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }
        public decimal Imdb { get; set; }
        public string MembershipName { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;

        public TvShowStatistics TvShowStatistics { get; set; } = null!;
        public List<GenreDto> Genres { get; set; } = new();
        public List<string> Tags { get; set; } = [];
        public List<PersonDto> People { get; set; } = new();

        public List<Season> Seasons { get; set; } = new();
    }
}
