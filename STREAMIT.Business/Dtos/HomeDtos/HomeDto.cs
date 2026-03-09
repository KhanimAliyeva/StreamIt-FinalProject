using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.SliderDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.HomeDtos
{
    public class HomeDto
    {
        public List<Slider2Dto> Sliders { get; set; } = [];
        public List<GetMovieDto> UpcomingMovies { get; set; } = [];
        public List<GetMovieDto> ContinueMovies { get; set; } = [];
    }
}

