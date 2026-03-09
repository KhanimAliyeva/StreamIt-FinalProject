using AutoMapper;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.TvShowDtos;
using STREAMIT.Core.Entities;
using System.Linq;

namespace STREAMIT.Business.Profiles
{
    public class TvShowProfile : Profile
    {
        public TvShowProfile()
        {
            // Create / Update DTO-lar
            CreateMap<CreateTvShowDto, TVShow>()
                .ForMember(dest => dest.TvShowGenres, opt => opt.Ignore())
                .ForMember(dest => dest.TvShowTags, opt => opt.Ignore())
                .ForMember(dest => dest.TvShowPeople, opt => opt.Ignore())
                .ForMember(dest => dest.Seasons, opt => opt.Ignore());

            CreateMap<UpdateTvShowDto, TVShow>()
                .ForMember(dest => dest.TvShowGenres, opt => opt.Ignore())
                .ForMember(dest => dest.TvShowTags, opt => opt.Ignore())
                .ForMember(dest => dest.TvShowPeople, opt => opt.Ignore())
                .ForMember(dest => dest.Seasons, opt => opt.Ignore());

            // Get DTO
            CreateMap<TVShow, GetTvShowDto>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(
                    src => src.TvShowGenres.Select(tg => new GenreDto
                    {
                        Id = tg.Genre!.Id,
                        Name = tg.Genre.Name
                    })
                ))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(
                    src => src.TvShowTags.Select(tt => tt.Tag!.Name)
                ))
                .ForMember(dest => dest.People, opt => opt.MapFrom(
                    src => src.TvShowPeople.Select(tp => new PersonDto
                    {
                        Id = tp.Person!.Id,
                        Name = tp.Person.Name,
                        ImageUrl = tp.Person.ImageUrl,
                        Role = tp.Role
                    })
                ))
                .ForMember(dest => dest.MembershipName, opt => opt.MapFrom(src => src.Membership != null ? src.Membership.Name : string.Empty))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty));
        }
    }
}
