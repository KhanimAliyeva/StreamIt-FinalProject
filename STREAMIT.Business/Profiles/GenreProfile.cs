using AutoMapper;
using STREAMIT.Business.Dtos.GenreDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Profiles
{
    internal class GenreProfile: Profile
    {
            public GenreProfile()
            {
                CreateMap<Core.Entities.Genre,CreateGenreDto>().ReverseMap();
                CreateMap<Core.Entities.Genre,UpdateGenreDto>().ReverseMap();
                CreateMap<Core.Entities.Genre,GetGenreDto>().ReverseMap();
        }
    }
}
