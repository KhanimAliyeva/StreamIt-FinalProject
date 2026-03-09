using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.PersonDtos
{
    public class CreatePersonDto
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Born { get; set; }
        public DateTime? Died { get; set; }
        public float Popularity { get; set; }
        public string Biography { get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;

        public IFormFile Image { get; set; } = null!;
    }
}
