using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.WatchlistDtos
{
    public class GetWatchListDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; }=string.Empty;
        public string PosterUrl { get; set; }= string.Empty;
    }
}
