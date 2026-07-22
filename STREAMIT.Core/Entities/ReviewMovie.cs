using STREAMIT.Core.Entities.Common;

namespace STREAMIT.Core.Entities
{
    public class ReviewMovie : BaseAuditableEntity
    {
        public int MovieId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        // Self-referencing for replies (null = top-level review)
        public int? ParentReviewId { get; set; }
        public ReviewMovie? ParentReview { get; set; }
        public ICollection<ReviewMovie> Replies { get; set; } = new List<ReviewMovie>();

        // Likes
        public int LikeCount { get; set; } = 0;
        public ICollection<ReviewLike> Likes { get; set; } = new List<ReviewLike>();

        public Movie? Movie { get; set; }
        public AppUser? User { get; set; }
    }
}
