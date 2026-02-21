using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.TvShowDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.GenreDtos
{
    public class GetGenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<GetMovieDto> Movies { get; set; } = [];
        public List<GetTvShowDto> TvShows { get; set; } = [];

    }
}
