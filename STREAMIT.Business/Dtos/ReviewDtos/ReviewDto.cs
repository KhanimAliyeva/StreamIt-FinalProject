using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.ReviewDtos
{
    public class ReviewDto
    {
        public string UserName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
