using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.MovieStaticticsDtos
{
    public class MovieStatisticsDto
    {
        public int ViewCount { get; set; } 
        public int LikeCount { get; set; }
        public decimal Rating { get; set; }
    }

}
