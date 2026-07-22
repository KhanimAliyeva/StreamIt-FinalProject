namespace STREAMIT.Business.Dtos.ReviewDtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int? ParentReviewId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsOwn { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int LikeCount { get; set; }
        public bool LikedByCurrentUser { get; set; }
        public List<ReviewDto> Replies { get; set; } = new();
    }
}
