using AutoMapper;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Core.Entities;

namespace STREAMIT.Business.Profiles
{
    public class LanguageProfile : Profile
    {
        public LanguageProfile()
        {
            CreateMap<Language, GetLanguageDto>().ReverseMap();
            CreateMap<CreateLanguageDto, Language>().ReverseMap();
            CreateMap<UpdateLanguageDto, Language>().ReverseMap();
        }
    }
}
