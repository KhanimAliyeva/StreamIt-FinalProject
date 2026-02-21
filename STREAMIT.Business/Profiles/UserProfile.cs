using AutoMapper;
using STREAMIT.Business.Dtos.UserDtos;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
           CreateMap<AppUser,RegisterDto>().ReverseMap();
        }
    }
}
