using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Profiles
{
    public class PersonProfile:Profile
    {
            public PersonProfile()
            {
                CreateMap<Core.Entities.Person, Dtos.PersonDtos.GetPersonDto>().ReverseMap();
                CreateMap<Core.Entities.Person, Dtos.PersonDtos.CreatePersonDto>().ReverseMap();
                CreateMap<Core.Entities.Person, Dtos.PersonDtos.UpdatePersonDto>().ReverseMap();
        }
    }
}
