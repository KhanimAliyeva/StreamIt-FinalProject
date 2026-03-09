using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.PersonDtos
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; 
    }

}
