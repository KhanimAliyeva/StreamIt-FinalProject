using AutoMapper;
using STREAMIT.Business.Dtos.LanguageDtos.STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Profiles
{
    public class MembershipProfile:Profile
    {
        public MembershipProfile()
        {

            CreateMap<Membership, GetMembershipDto>().ReverseMap();
            CreateMap<Membership, CreateMembershipDto>().ReverseMap();
            CreateMap<Membership, UpdateMembershipDto>().ReverseMap();
            
        }
    }
}
