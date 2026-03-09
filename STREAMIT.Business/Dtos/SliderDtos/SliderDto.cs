using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.SliderDtos
{
    public class SliderDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Title { get; set; }=string.Empty;
        public string CoverImageUrl { get; set; }= string.Empty;
        public string Description { get; set; }= string.Empty;
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Movie? Movie { get; set; }
    }
}
