using STREAMIT.Core.Entities.Common;

namespace STREAMIT.Core.Entities
{
    public class ReviewLike : BaseEntity
    {
        public int ReviewId { get; set; }
        public ReviewMovie? Review { get; set; }

        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
    }
}
