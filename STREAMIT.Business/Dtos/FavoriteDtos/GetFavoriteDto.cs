using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.FavoriteDtos
{
    public class GetFavoriteDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
    }
}
