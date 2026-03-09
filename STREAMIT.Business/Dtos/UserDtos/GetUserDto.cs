using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.UserDtos
{
    public class GetUserDto
    {
        public string Id { get; set; }=string.Empty;
        public string UserName { get; set; }= string.Empty;
        public string Email { get; set; }=  string.Empty;

        public string Fullname { get; set; }= string.Empty;
        public string? ProfilePictureUrl { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Role { get; set; }= string.Empty;

        public string? MembershipName { get; set; }

        public int FavoriteMoviesCount { get; set; }

        public int MovieReviewsCount { get; set; }
    }
}
