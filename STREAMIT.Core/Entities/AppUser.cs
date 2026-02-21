using Microsoft.AspNetCore.Identity;
using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class AppUser : IdentityUser

    {
        public string Fullname { get; set; }=string.Empty;
        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiredDate { get; set; }


        public ICollection<UserMovie> UserMovies { get; set; } = [];
        public ICollection<UserTvShow> UserTvShows { get; set; }=[];
        public ICollection<ReviewMovie> MovieReviews { get; set; } = [];
        public ICollection<ReviewTvShow> TvShowReviews { get; set; } = [];
        public ICollection<UserMembership> UserMemberships { get; set; } = [];

    }

}
