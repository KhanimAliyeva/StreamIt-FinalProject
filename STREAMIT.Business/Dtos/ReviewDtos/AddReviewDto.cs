using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.ReviewDtos
{
    public class AddReviewDto
    {
        public int MovieId { get; set; }     
        public string UserId { get; set; }
        
        
        public decimal Rating { get; set; }     
        public string Comment { get; set; } = ""; 
    }
}
