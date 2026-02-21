using STREAMIT.Business.Dtos;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Dtos.MovieStaticticsDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.MovieDtos
{

    public class GetMovieDto
    {

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public string MovieUrl { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public float Duration { get; set; }
        public decimal Imdb { get; set; }
        public string Status { get; set; } = string.Empty;


        public string MembershipName { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;

        public MovieStatisticsDto? MovieStatistics { get; set; }

        public List<GenreDto> Genres { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public List<PersonDto> People { get; set; } = new();
    }

}






