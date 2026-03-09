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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfilePictureUrl { get; set; }



        public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
        public ICollection<ReviewMovie> MovieReviews { get; set; } = new List<ReviewMovie>();
        public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();

    }

}
