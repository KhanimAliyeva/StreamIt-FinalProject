using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Movie : BaseAuditableEntity
    {
        public string Title { get; set; }=string.Empty;
        public string Status { get; set; }=string.Empty;
        public string Content { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string MovieUrl { get; set; } = string.Empty;
        public float Duration { get; set; }
        public int MembershipId { get; set; }
        public string TrailerUrl { get; set; } = string.Empty;
        public int LanguageId { get; set; }
        public decimal Imdb { get; set; }

        public Membership? Membership { get; set; }
        public Language? Language { get; set; }

        public MovieStatistics? MovieStatistics { get; set; } 
        public ICollection<MovieGenre> MovieGenres { get; set; } = [];
        public ICollection<MovieTag> MovieTags { get; set; }= [];
        public ICollection<MoviePerson> MoviePeople { get; set; } = [];
        public ICollection<ReviewMovie> Reviews { get; set; } = [];
        public ICollection<UserMovie> UserMovies { get; set; } = [];
    }

}
