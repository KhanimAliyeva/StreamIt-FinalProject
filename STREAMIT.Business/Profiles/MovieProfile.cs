using AutoMapper;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.MovieStaticticsDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static STREAMIT.Business.Dtos.MovieDtos.GetMovieDto;

namespace STREAMIT.Business.Profiles
{

    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, GetMovieDto>()
     .ForMember(dest => dest.Genres, opt => opt.MapFrom(
         src => src.MovieGenres.Select(mg => new GenreDto
         {
             Id = mg.Genre!.Id,
             Name = mg.Genre.Name
         })
     ))
     .ForMember(dest => dest.Tags, opt => opt.MapFrom(
         src => src.MovieTags.Select(mt => mt.Tag!.Name)))
     .ForMember(dest => dest.People, opt => opt.MapFrom(
         src => src.MoviePeople.Select(mp => new PersonDto
         {
             Id = mp.Person!.Id,
             Name = mp.Person.Name,
             ImageUrl = mp.Person.ImageUrl,   // <-- əlavə et
             Role = mp.Role
         })))
     .ForMember(dest => dest.MembershipName,
                opt => opt.MapFrom(src => src.Membership!.Name));


            CreateMap<CreateMovieDto, Movie>()
     .ForMember(dest => dest.MoviePeople, opt => opt.Ignore())
     .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
     .ForMember(dest => dest.MovieTags, opt => opt.Ignore());

            CreateMap<UpdateMovieDto, Movie>()
                .ForMember(dest => dest.MoviePeople, opt => opt.Ignore())
                .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
                .ForMember(dest => dest.MovieTags, opt => opt.Ignore());
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<Language, LanguageDto>().ReverseMap();
            CreateMap<Membership, MembershipDto>().ReverseMap();
            CreateMap<MovieStatistics, MovieStatisticsDto>().ReverseMap();
        }
    }

}

