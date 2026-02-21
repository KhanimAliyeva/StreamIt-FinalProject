using AutoMapper;
using STREAMIT.Business.Dtos.TagDtos.STREAMIT.Business.Dtos.TagDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Profiles
{
    public class TagProfile:Profile
    {
        public TagProfile()
        {
                CreateMap<Core.Entities.Tag, GetTagDto>().ReverseMap();
                CreateMap<Core.Entities.Tag, CreateTagDto>().ReverseMap();
                CreateMap<Core.Entities.Tag, UpdateTagDto>().ReverseMap();
        }
    }
}
