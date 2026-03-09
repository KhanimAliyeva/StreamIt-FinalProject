using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.RatingDtos
{
    public class RatingDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty; // Movie or Show
        public string Name { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
