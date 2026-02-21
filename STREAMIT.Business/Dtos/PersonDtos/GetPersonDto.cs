using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.PersonDtos
{
    public class GetPersonDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Born { get; set; }
        public DateTime? Died { get; set; }
        public float Popularity { get; set; }
        public string Biography { get; set; } = string.Empty;
       
    }
}
