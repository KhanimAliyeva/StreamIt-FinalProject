using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class TVShow : BaseAuditableEntity
    {
        public string Title { get; set; }=string.Empty;
        public int MembershipId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public int LanguageId { get; set; }
        public decimal Imdb { get; set; }
        public string TrailerUrl { get; set; } = string.Empty;

        public Membership? Membership { get; set; }
        public Language? Language { get; set; }

        public ICollection<Season> Seasons { get; set; } = [];


        public TvShowStatistics? TvShowStatistics { get; set; }
        public ICollection<TvShowGenre> TvShowGenres { get; set; } = [];
        public ICollection<TvShowTag> TvShowTags { get; set; } = [];
        public ICollection<TvShowPerson> TvShowPeople { get; set; } = [];
        public ICollection<ReviewTvShow> Reviews { get; set; } = [];
        public ICollection<UserTvShow> UserTvShows { get; set; } = [];
    }

}
